using AutoMapper;
using GymDAL.Repo.Abstract.Extra;
using System.Linq.Expressions;

namespace GymDAL.Repo.Implementation
{
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {

        public NotificationRepository(GymDbContext context) : base(context)
        {
        }

    }
}