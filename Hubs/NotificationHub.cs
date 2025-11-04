using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AuctionPortal.Hubs
{
    [Authorize] 
    public class NotificationHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
    }
}
