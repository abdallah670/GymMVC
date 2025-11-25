using GymBLL.ModelVM.External;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymBLL.Service.Abstract
{
    public interface ISubscriptionService
    {
        Task<Response<SubscriptionVM>> CreateAsync(SubscriptionVM model);
        Task<Response<(bool hasWorkout, bool hasDiet)>> CheckActivePlanAsync(string memberId);
        Task<Response<SubscriptionDetailsVM>> GetByIdAsync(int id);
        Task<Response<SubscriptionDetailsVM>> GetByMemeberIdAsync(string id);
        Task<Response<SubscriptionVM>> GetMembershipAsync(int id);
        Task<Response<List<SubscriptionVM>>> GetAllAsync();
        Task<Response<List<SubscriptionVM>>> GetByMemberIdAsync(string memberId);
        Task<Response<List<SubscriptionVM>>> GetActiveSubscriptionsAsync();
        Task<Response<SubscriptionVM>> UpdateAsync(SubscriptionVM model);
        Task<Response<SubscriptionDetailsVM>> UpdateAsync(SubscriptionDetailsVM model);
        Task<Response<bool>> CancelSubscriptionAsync(int id);
        Task<Response<bool>> RenewSubscriptionAsync(int id);
        Task<Response<bool>> DeleteAsync(int id);
        Task<Response<List<SubscriptionVM>>> GetExpiringSubscriptionsAsync(int daysUntilExpiry);
        Task<Response<List<SubscriptionVM>>> GetExpiredSubscriptionsAsync();
        Task<Response<SubscriptionVM>> GetActiveSubscriptionByMemberIdAsync(string memberId);
        Task<Response<SubscriptionVM>> UpgradeSubscriptionAsync(int currentSubscriptionId, int newMembershipId, PaymentVM payment);
    }
}
