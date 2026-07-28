namespace RealtimeService.Api.Hubs;

using Microsoft.AspNetCore.SignalR;

public class AuctionHub : Hub
{
    public async Task JoinAuctionGroup(string auctionId)
    {
        try
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, auctionId);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task LeaveAuctionGroup(string auctionId)
    {
        try
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, auctionId);
        }
        catch (Exception)
        {
            throw;
        }
    }
}