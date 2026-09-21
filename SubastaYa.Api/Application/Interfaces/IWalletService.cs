using Application.Common;
using Application.DTOs.Wallet;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IWalletService
    {
        Task<Result<WalletBalanceDto>> GetBalanceAsync(int userId, CancellationToken ct = default);
        Task<Result<WalletBalanceDto>> DepositAsync(int userId, decimal amount, CancellationToken ct = default);
        Task<Result<IReadOnlyList<LedgerTransactionDto>>> GetTransactionsAsync(int userId, CancellationToken ct = default);

    }
}
