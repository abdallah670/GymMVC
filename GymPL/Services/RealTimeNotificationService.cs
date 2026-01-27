using GymBLL.Service.Abstract.Communication;
using GymPL.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace GymPL.Services
{
    public class RealTimeNotificationService : IRealTimeNotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public RealTimeNotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task PushNotificationAsync(string userId, string message)
        {
            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", message);
        }
    }
}
