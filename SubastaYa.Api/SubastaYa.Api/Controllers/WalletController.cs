using Application.DTOs.Wallet;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SubastaYa.Api.Controllers
{
    public class WalletController : ApiControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService) => _walletService = walletService;

        // GET /api/wallet/balance
        [HttpGet("balance")]
        public async Task<ActionResult<WalletBalanceDto>> GetBalance(CancellationToken ct)
        {
            var userId = GetUserId();
            var result = await _walletService.GetBalanceAsync(userId, ct);
            return HandleResult(result);
        }

        // POST /api/wallet/deposit
        [HttpPost("deposit")]
        public async Task<ActionResult<WalletBalanceDto>> Deposit(
            [FromBody] DepositRequest request, CancellationToken ct)
        {
            var userId = GetUserId();
            var result = await _walletService.DepositAsync(userId, request.Amount, ct);
            return HandleResult(result);
        }

        // GET /api/wallet/transactions 
        [HttpGet("transactions")]
        public async Task<ActionResult<IReadOnlyList<LedgerTransactionDto>>> GetTransactions(CancellationToken ct)
        {
            var userId = GetUserId();
            var result = await _walletService.GetTransactionsAsync(userId, ct);
            return HandleResult(result);
        }
    }
}
