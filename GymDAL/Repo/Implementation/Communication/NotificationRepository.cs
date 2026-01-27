using AutoMapper;
using GymDAL.Repo.Abstract.Communication;
using System.Linq.Expressions;
using GymDAL.Entities.Communication;

namespace GymDAL.Repo.Implementation.Communication
{
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {

        public NotificationRepository(GymDbContext context) : base(context)
        {
        }

    }
}