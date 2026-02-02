using GymBLL.ModelVM.Financial;
using GymBLL.ModelVM;
using GymBLL.Response;
using GymBLL.ModelVM.Financial;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace GymBLL.Service.Abstract.Financial { 
    public interface ISubscriptionService 
    { Task<Response<SubscriptionVM>> CreateAsync(SubscriptionVM model); 
       Task<Response<(bool hasWorkout, int? workoutAssignmentId, bool hasDiet, int? dietPlanAssignmentId)>> CheckActivePlanAsync(string memberId);
        Task<Response<SubscriptionDetailsVM>> GetByIdAsync(int id);
        Task<Response<SubscriptionDetailsVM>> GetByMemeberIdAsync(string id); 
        Task<Response<SubscriptionVM>> GetMembershipAsync(int id); 
        Task<Response<List<SubscriptionVM>>> GetAllAsync();
        Task<Response<List<SubscriptionVM>>> GetByMemberIdAsync(string memberId);
        Task<Response<SubscriptionDetailsVM>> GetSubscritionDietByMemberIdAsync(string memberId);
        Task<Response<SubscriptionDetailsVM>> GetSubscritionWorkoutByMemberIdAsync(string memberId);
        Task<Response<List<SubscriptionVM>>> GetActiveSubscriptionsAsync();
        Task<Response<SubscriptionVM>> UpdateAsync(SubscriptionVM model); 
        Task<Response<SubscriptionDetailsVM>> UpdateAsync(SubscriptionDetailsVM model);
        Task<Response<bool>> CancelSubscriptionAsync(int id);
        Task<Response<bool>> RenewSubscriptionAsync(int id, PaymentVM? payment = null);
        Task<Response<bool>> DeleteAsync(int id); Task<Response<List<SubscriptionVM>>> GetExpiringSubscriptionsAsync(int daysUntilExpiry);
        Task<Response<List<SubscriptionVM>>> GetExpiredSubscriptionsAsync(); 
        Task<Response<SubscriptionVM>> GetActiveSubscriptionByMemberIdAsync(string memberId);
        Task<Response<SubscriptionVM>> UpgradeSubscriptionAsync(int currentSubscriptionId, int newMembershipId, PaymentVM payment);
        Task<Response<int>> GetAssignedDietPlan(string MemberId);
    } }
