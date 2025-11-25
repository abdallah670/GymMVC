using GymDAL.Entities.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymDAL.Repo.Abstract.Extra
{
    public interface ISubscriptionRepository: IRepository<Subscription>
    {
        Task <Subscription> GetMembershipByIdAsync(int Id);

    }
}
