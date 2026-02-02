using GymDAL.Entities.Financial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymDAL.Repo.Abstract.Financial
{
    public interface ISubscriptionRepository: IRepository<Subscription>
    {
        Task<Subscription> GetActiveSubscriptionByMemberIdAsync(string userId);
        Task<Subscription> GetMembershipByIdAsync(int Id);

    }
}
