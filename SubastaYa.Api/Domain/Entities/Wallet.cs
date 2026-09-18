using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Wallet
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public decimal TotalBalance { get; set; }

        public decimal HeldBalance { get; set; }

        public decimal AvailableBalance => TotalBalance - HeldBalance;

        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        public virtual User User { get; set; } = null!;

        public virtual ICollection<LedgerTransaction> LedgerTransactions { get; set; } = new List<LedgerTransaction>();
    }
}
