using Application.Common;
using Application.DTOs.Auctions;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class BiddingService : IBiddingService
    {
        private readonly AppDbContext _context;
        private readonly AuditService _audit;

        private static readonly TimeSpan AntiSnipingWindow = TimeSpan.FromSeconds(60);
        private static readonly TimeSpan AntiSnipingExtension = TimeSpan.FromMinutes(2);
        private readonly IAuctionNotifier _notifier;

        public BiddingService(AppDbContext context, AuditService audit, IAuctionNotifier notifier)
        {
            _context = context;
            _audit = audit;
            _notifier = notifier;
        }

        public async Task<Result<BidDto>> PlaceBidAsync(
            int auctionId, int userId, decimal amount, CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;

            
            var auction = await _context.Auctions.FirstOrDefaultAsync(a => a.Id == auctionId, ct);
            if (auction is null)
            {
                return Result<BidDto>.Failure(ErrorType.NotFound, "La subasta no existe.");
            }

            
            if (auction.Status != AuctionStatus.Active || now < auction.StartDate || now >= auction.EndDate)
            {
                return Result<BidDto>.Failure(ErrorType.Conflict, "La subasta no está disponible para pujar.");
            }

            
            var highestBid = await _context.Bids
                .Where(b => b.AuctionId == auctionId)
                .OrderByDescending(b => b.Amount)
                .FirstOrDefaultAsync(ct);

            var currentAmount = highestBid?.Amount ?? auction.BasePrice;
            var minimumValid = highestBid is null
                ? auction.BasePrice
                : currentAmount + auction.MinimumIncrement;

            
            if (highestBid is not null && highestBid.UserId == userId)
            {
                return Result<BidDto>.Failure(ErrorType.Conflict, "Ya sos el postor líder.");
            }

           
            if (amount < minimumValid)
            {
                return Result<BidDto>.Failure(
                    ErrorType.Validation,
                    $"El monto debe ser al menos {minimumValid}.");
            }

            
            var bidderWallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId, ct);
            if (bidderWallet is null)
            {
                return Result<BidDto>.Failure(ErrorType.NotFound, "El usuario no tiene billetera.");
            }

            
            if (bidderWallet.AvailableBalance < amount)
            {
               
                _audit.Register("Bid", auctionId, "BidRejectedInsufficientFunds", userId,
                    new { attemptedAmount = amount, available = bidderWallet.AvailableBalance });
                await _context.SaveChangesAsync(ct);

                return Result<BidDto>.Failure(ErrorType.InsufficientFunds, "Saldo disponible insuficiente.");
            }

           
            await using var transaction = await _context.Database.BeginTransactionAsync(ct);
            try
            {
                
                if (highestBid is not null)
                {
                    var previousWallet = await _context.Wallets
                        .FirstOrDefaultAsync(w => w.UserId == highestBid.UserId, ct);

                    if (previousWallet is not null)
                    {
                        previousWallet.HeldBalance -= highestBid.Amount;

                        _context.LedgerTransactions.Add(new LedgerTransaction
                        {
                            WalletId = previousWallet.Id,
                            Type = LedgerTransactionType.Release,
                            Amount = highestBid.Amount,
                            CreatedAt = now,
                            AuctionId = auctionId
                        });
                    }
                }

                
                bidderWallet.HeldBalance += amount;

                _context.LedgerTransactions.Add(new LedgerTransaction
                {
                    WalletId = bidderWallet.Id,
                    Type = LedgerTransactionType.Hold,
                    Amount = amount,
                    CreatedAt = now,
                    AuctionId = auctionId
                });

               
                var bid = new Bid
                {
                    AuctionId = auctionId,
                    UserId = userId,
                    Amount = amount,
                    BidDate = now
                };
                _context.Bids.Add(bid);

              
                var extended = false;
                if (auction.EndDate - now <= AntiSnipingWindow)
                {
                    auction.EndDate = auction.EndDate.Add(AntiSnipingExtension);
                    extended = true;
                    _audit.Register("Auction", auctionId, "AntiSnipingExtension", userId,
                        new { newEndDate = auction.EndDate });
                }

                
                _context.Entry(auction).Property(a => a.EndDate).IsModified = true;

                
                await _context.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
                await _notifier.BidPlacedAsync(auctionId, amount, BuildAlias(userId), now);

                if (extended)
                {
                    await _notifier.AuctionExtendedAsync(auctionId, auction.EndDate);
                }

                var dto = new BidDto
                {
                    Id = bid.Id,
                    Amount = bid.Amount,
                    BidDate = bid.BidDate,
                    BidderAlias = BuildAlias(userId)
                };


                return Result<BidDto>.Success(dto);
            }
            catch (DbUpdateConcurrencyException)
            {
                
                await transaction.RollbackAsync(ct);

               
                _audit.Register("Bid", auctionId, "BidRejectedConcurrency", userId,
                    new { attemptedAmount = amount });
                await _context.SaveChangesAsync(ct);

                return Result<BidDto>.Failure(
                    ErrorType.Conflict, "Otra puja se registró primero. Intentá nuevamente.");
            }
        }

        public async Task<IReadOnlyList<BidDto>> GetBidsAsync(int auctionId, CancellationToken ct = default)
        {
            return await _context.Bids
                .Where(b => b.AuctionId == auctionId)
                .OrderByDescending(b => b.BidDate)
                .Select(b => new BidDto
                {
                    Id = b.Id,
                    Amount = b.Amount,
                    BidDate = b.BidDate,
                    BidderAlias = "User#" + b.UserId
                })
                .ToListAsync(ct);
        }

        private static string BuildAlias(int userId) => "User#" + userId;
    }
}
