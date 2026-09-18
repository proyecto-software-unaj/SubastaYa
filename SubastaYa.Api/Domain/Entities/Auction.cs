using Domain.Enums;
using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Auction
    {
        public int Id { get; set; }

        public int SellerId { get; set; }

        public int CategoryId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public decimal BasePrice { get; set; }

        public decimal MinimumIncrement { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public AuctionStatus Status { get; set; }

        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        public virtual User Seller { get; set; } = null!;

        public virtual Category Category { get; set; } = null!;

        public virtual ICollection<Bid> Bids { get; set; } = new List<Bid>();

        public virtual ICollection<LedgerTransaction> LedgerTransactions { get; set; } = new List<LedgerTransaction>();
    }
}
