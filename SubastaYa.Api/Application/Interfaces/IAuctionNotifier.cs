using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IAuctionNotifier
    {
        Task BidPlacedAsync(int auctionId, decimal amount, string bidderAlias, DateTime bidDate);
        Task AuctionExtendedAsync(int auctionId, DateTime newEndDate);
    }
}
