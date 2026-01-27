using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace GymPL.Hubs
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            if (Context.User?.Identity?.IsAuthenticated == true)
            {
                // Optionally add to a group named after the username or ID
                var userId = Context.UserIdentifier;
                if (!string.IsNullOrEmpty(userId))
                {
                   await Groups.AddToGroupAsync(Context.ConnectionId, userId);
                }
            }
            await base.OnConnectedAsync();
        }

        public async Task SendNotification(string userId, string message)
        {
            await Clients.User(userId).SendAsync("ReceiveNotification", message);
        }
    }
}
