using System.Threading.Tasks;

namespace GymBLL.Service.Abstract.Communication
{
    public interface IRealTimeNotificationService
    {
        Task PushNotificationAsync(string userId, string message);
    }
}
