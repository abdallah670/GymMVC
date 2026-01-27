using GymBLL.ModelVM.Communication;
namespace GymBLL.Service.Abstract.Communication
{
    public interface INotificationService
    {
        Task<Response<NotificationVM>> CreateAsync(NotificationVM model);
        Task<Response<NotificationVM>> GetByIdAsync(int id);
        Task<Response<List<NotificationVM>>> GetAllAsync();
        Task<Response<List<NotificationVM>>> GetByUserIdAsync(string userId);
        Task<Response<List<NotificationVM>>> GetUnreadByUserIdAsync(string userId); 
        Task<Response<NotificationVM>> UpdateAsync(NotificationVM model); 
        Task<Response<bool>> MarkAsReadAsync(int id); 
        Task<Response<bool>> DeleteAsync(int id); 
        Task<Response<bool>> ToggleStatusAsync(int id);
    }
}
