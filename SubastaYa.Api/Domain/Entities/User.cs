using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public DateTime RegisteredAt { get; set; }

        public virtual Wallet Wallet { get; set; } = null!;

        public virtual ICollection<Auction> Auctions { get; set; } = new List<Auction>();

        public virtual ICollection<Bid> Bids { get; set; } = new List<Bid>();

        public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
    }
}
