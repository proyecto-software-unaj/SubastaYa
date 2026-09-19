using Microsoft.AspNetCore.SignalR;


namespace SubastaYa.Api.Hubs
{
    public class AuctionHub : Hub
    {
        private static string GroupName(int auctionId) => $"auction-{auctionId}";

        
        public Task JoinAuction(int auctionId) =>
            Groups.AddToGroupAsync(Context.ConnectionId, GroupName(auctionId));

        
        public Task LeaveAuction(int auctionId) =>
            Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(auctionId));
    }
}
