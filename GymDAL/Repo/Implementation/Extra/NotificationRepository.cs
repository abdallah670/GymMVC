using AutoMapper;
using GymDAL.Repo.Abstract.Extra;
using System.Linq.Expressions;

namespace GymDAL.Repo.Implementation
{
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        public NotificationRepository(GymDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public Task<Notification> CreateAsync(Notification notification, string Createdby)
        {
            try
            {
                notification.CreatedBy = Createdby;
                notification.CreatedAt = DateTime.UtcNow;
                _context.Notifications.Add(notification);
                _context.SaveChanges();
                return Task.FromResult(notification);
            }
            catch (Exception)
            {
                return Task.FromResult<Notification>(null);
            }
        }

        public Task<IEnumerable<Notification>> GetAsync(Expression<Func<Notification, bool>>? Filter)
        {
            try
            {
                var query = _context.Notifications.AsQueryable();
                if (Filter != null)
                {
                    query = query.Where(Filter);
                }
                return Task.FromResult(query.AsEnumerable());
            }
            catch (Exception)
            {
                return Task.FromResult<IEnumerable<Notification>>(null);
            }
        }

        public Task<Notification> GetByAsync(Expression<Func<Notification, bool>>? Filter)
        {
            try
            {
                if (Filter != null)
                {
                    var result = _context.Notifications.FirstOrDefault(Filter);
                    return Task.FromResult(result);
                }
                return Task.FromResult<Notification>(null);
            }
            catch (Exception)
            {
                return Task.FromResult<Notification>(null);
            }
        }

        public Task<bool> ToggleStatusAsync(int notificationId, string DeletedBy)
        {
            try
            {
                var notification = _context.Notifications.FirstOrDefault(n => n.Id == notificationId);
                if (notification != null)
                {
                    notification.ToggleStatus(DeletedBy);
                    _context.SaveChanges();
                    return Task.FromResult(true);
                }
                return Task.FromResult(false);
            }
            catch (Exception)
            {
                return Task.FromResult(false);
            }
        }

        public Task<Notification> UpdateAsync(Notification notification, string UpdatedBy)
        {
            try
            {
                var existingNotification = _context.Notifications.FirstOrDefault(n => n.Id == notification.Id);
                if (existingNotification != null)
                {
                    _mapper.Map(notification, existingNotification);
                    existingNotification.UpdatedBy = UpdatedBy;
                    existingNotification.UpdatedAt = DateTime.UtcNow;
                    _context.SaveChanges();
                    return Task.FromResult(existingNotification);
                }
                return Task.FromResult<Notification>(null);
            }
            catch (Exception)
            {
                return Task.FromResult<Notification>(null);
            }
        }
    }
}