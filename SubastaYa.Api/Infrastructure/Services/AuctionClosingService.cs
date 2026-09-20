using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class AuctionClosingService
    {
        private readonly AppDbContext _context;
        private readonly AuditService _audit;

        public AuctionClosingService(AppDbContext context, AuditService audit)
        {
            _context = context;
            _audit = audit;
        }

        
        public async Task<int> CloseExpiredAuctionsAsync(CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;

            
            var expired = await _context.Auctions
                .Where(a => a.Status == AuctionStatus.Active && a.EndDate <= now)
                .ToListAsync(ct);

            var closedCount = 0;

            foreach (var auction in expired)
            {
               
                await using var transaction = await _context.Database.BeginTransactionAsync(ct);
                try
                {
                    var winningBid = await _context.Bids
                        .Where(b => b.AuctionId == auction.Id)
                        .OrderByDescending(b => b.Amount)
                        .FirstOrDefaultAsync(ct);

                    if (winningBid is null)
                    {
                        
                        auction.Status = AuctionStatus.Deserted;
                        _audit.Register("Auction", auction.Id, "AuctionDeserted", null,
                            new { closedAt = now });
                    }
                    else
                    {
                        
                        auction.Status = AuctionStatus.Finished;

                        var buyerWallet = await _context.Wallets
                            .FirstOrDefaultAsync(w => w.UserId == winningBid.UserId, ct);
                        var sellerWallet = await _context.Wallets
                            .FirstOrDefaultAsync(w => w.UserId == auction.SellerId, ct);

                        if (buyerWallet is not null && sellerWallet is not null)
                        {

                            buyerWallet.HeldBalance =
                            Math.Max(0m, buyerWallet.HeldBalance - winningBid.Amount);
                            buyerWallet.TotalBalance -= winningBid.Amount;


                            sellerWallet.TotalBalance += winningBid.Amount;
                      

                            _context.LedgerTransactions.Add(new LedgerTransaction
                            {
                                WalletId = buyerWallet.Id,
                                Type = LedgerTransactionType.Payment,
                                Amount = winningBid.Amount,
                                CreatedAt = now,
                                AuctionId = auction.Id
                            });

                            _context.LedgerTransactions.Add(new LedgerTransaction
                            {
                                WalletId = sellerWallet.Id,
                                Type = LedgerTransactionType.Collection,
                                Amount = winningBid.Amount,
                                CreatedAt = now,
                                AuctionId = auction.Id
                            });
                        }

                        _audit.Register("Auction", auction.Id, "AuctionFinished", winningBid.UserId,
                            new { winnerId = winningBid.UserId, amount = winningBid.Amount });
                    }

                    await _context.SaveChangesAsync(ct);
                    await transaction.CommitAsync(ct);
                    closedCount++;
                }
                catch (DbUpdateConcurrencyException)
                {
                    
                    await transaction.RollbackAsync(ct);
                }
            }

            return closedCount;
        }

        public async Task<int> ActivateScheduledAuctionsAsync(CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;

            
            var toActivate = await _context.Auctions
                .Where(a => a.Status == AuctionStatus.Scheduled
                            && a.StartDate <= now
                            && a.EndDate > now)
                .ToListAsync(ct);

            var activatedCount = 0;

            foreach (var auction in toActivate)
            {
                await using var transaction = await _context.Database.BeginTransactionAsync(ct);
                try
                {
                    auction.Status = AuctionStatus.Active;
                    _audit.Register("Auction", auction.Id, "AuctionActivated", null,
                        new { activatedAt = now });

                    await _context.SaveChangesAsync(ct);
                    await transaction.CommitAsync(ct);
                    activatedCount++;
                }
                catch (DbUpdateConcurrencyException)
                {
                    
                    await transaction.RollbackAsync(ct);
                }
            }

            return activatedCount;
        }
    }
}
