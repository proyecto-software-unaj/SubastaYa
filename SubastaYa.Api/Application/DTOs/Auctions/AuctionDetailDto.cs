using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Auctions
{
    public class AuctionDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public decimal MinimumIncrement { get; set; }
        public decimal CurrentHighestBid { get; set; }
        public int? CurrentWinnerId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal SuggestedNextBid { get; set; }
        public List<BidDto> RecentBids { get; set; } = new();
    }
}
