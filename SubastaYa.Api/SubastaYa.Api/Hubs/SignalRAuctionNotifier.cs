using Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace SubastaYa.Api.Hubs
{
    public class SignalRAuctionNotifier : IAuctionNotifier
    {
        private readonly IHubContext<AuctionHub> _hub;

        public SignalRAuctionNotifier(IHubContext<AuctionHub> hub) => _hub = hub;

        private static string GroupName(int auctionId) => $"auction-{auctionId}";

        public Task BidPlacedAsync(int auctionId, decimal amount, string bidderAlias, DateTime bidDate)
        {
            
            return _hub.Clients.Group(GroupName(auctionId)).SendAsync("BidPlaced", new
            {
                auctionId,
                amount,
                bidderAlias,
                bidDate
            });
        }

        public Task AuctionExtendedAsync(int auctionId, DateTime newEndDate)
        {
            return _hub.Clients.Group(GroupName(auctionId)).SendAsync("AuctionExtended", new
            {
                auctionId,
                newEndDate
            });
        }
    }
}
