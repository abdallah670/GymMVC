using System.Linq.Expressions;

namespace GymDAL.Repo.Abstract.Extra
{
    public interface INotificationRepository : IRepository<Notification>
    {
        // GET Operations
        Task<Notification> GetByAsync(Expression<Func<Notification, bool>>? Filter);
        Task<IEnumerable<Notification>> GetAsync(Expression<Func<Notification, bool>>? Filter);

        // CREATE Operations
        Task<Notification> CreateAsync(Notification notification, string Createdby);

        // UPDATE Operations
        Task<Notification> UpdateAsync(Notification notification, string UpdatedBy);

        // DELETE Operations
        Task<bool> ToggleStatusAsync(int notificationId, string DeletedBy);
    }
}