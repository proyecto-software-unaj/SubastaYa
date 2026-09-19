using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Auctions
{
    public class BidDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime BidDate { get; set; }
        public string BidderAlias { get; set; } = string.Empty;
    }
}
