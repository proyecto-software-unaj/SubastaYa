using Domain.Enums;
using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class LedgerTransaction
    {
        public int Id { get; set; }

        public int WalletId { get; set; }

        public LedgerTransactionType Type { get; set; }

        public decimal Amount { get; set; }

        public DateTime CreatedAt { get; set; }

        public int? AuctionId { get; set; }

        public virtual Wallet Wallet { get; set; } = null!;

        public virtual Auction? Auction { get; set; }
    }
}
