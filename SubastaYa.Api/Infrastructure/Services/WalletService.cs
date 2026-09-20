using Application.Common;
using Application.DTOs.Wallet;
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
    public class WalletService : IWalletService
    {
        private readonly AppDbContext _context;
        private readonly AuditService _audit;

        public WalletService(AppDbContext context, AuditService audit)
        {
            _context = context;
            _audit = audit;
        }

        public async Task<Result<WalletBalanceDto>> GetBalanceAsync(int userId, CancellationToken ct = default)
        {
            var wallet = await _context.Wallets
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.UserId == userId, ct);

            if (wallet is null)
            {
                return Result<WalletBalanceDto>.Failure(ErrorType.NotFound, "El usuario no tiene billetera.");
            }

            var dto = new WalletBalanceDto
            {
                TotalBalance = wallet.TotalBalance,
                HeldBalance = wallet.HeldBalance,
                AvailableBalance = wallet.AvailableBalance
            };

            return Result<WalletBalanceDto>.Success(dto);
        }

        public async Task<Result<WalletBalanceDto>> DepositAsync(int userId, decimal amount, CancellationToken ct = default)
        {
            
            if (amount <= 0)
            {
                return Result<WalletBalanceDto>.Failure(ErrorType.Validation, "El monto a depositar debe ser positivo.");
            }

            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId, ct);
            if (wallet is null)
            {
                return Result<WalletBalanceDto>.Failure(ErrorType.NotFound, "El usuario no tiene billetera.");
            }

            var now = DateTime.UtcNow;

            await using var transaction = await _context.Database.BeginTransactionAsync(ct);
            try
            {
               
                wallet.TotalBalance += amount;

               
                _context.LedgerTransactions.Add(new LedgerTransaction
                {
                    WalletId = wallet.Id,
                    Type = LedgerTransactionType.Deposit,
                    Amount = amount,
                    CreatedAt = now
                });

                
                _audit.Register("Wallet", wallet.Id, "ManualCredit", userId, new { amount });

                await _context.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);

                var dto = new WalletBalanceDto
                {
                    TotalBalance = wallet.TotalBalance,
                    HeldBalance = wallet.HeldBalance,
                    AvailableBalance = wallet.AvailableBalance
                };

                return Result<WalletBalanceDto>.Success(dto);
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync(ct);
                return Result<WalletBalanceDto>.Failure(
                    ErrorType.Conflict, "La billetera fue modificada por otra operación. Intentá nuevamente.");
            }
        }

        public async Task<Result<IReadOnlyList<LedgerTransactionDto>>> GetTransactionsAsync(
            int userId, CancellationToken ct = default)
        {
            var wallet = await _context.Wallets
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.UserId == userId, ct);

            if (wallet is null)
            {
                return Result<IReadOnlyList<LedgerTransactionDto>>.Failure(
                    ErrorType.NotFound, "El usuario no tiene billetera.");
            }

            var transactions = await _context.LedgerTransactions
                .AsNoTracking()
                .Where(t => t.WalletId == wallet.Id)
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new LedgerTransactionDto
                {
                    Id = t.Id,
                    Type = t.Type.ToString(),
                    Amount = t.Amount,
                    CreatedAt = t.CreatedAt,
                    AuctionId = t.AuctionId
                })
                .ToListAsync(ct);

            return Result<IReadOnlyList<LedgerTransactionDto>>.Success(transactions);
        }

    }
}
