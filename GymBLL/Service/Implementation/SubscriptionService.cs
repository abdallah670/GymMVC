using AutoMapper;
using GymBLL.ModelVM.External;
using GymBLL.Service.Abstract;
using GymDAL.Entities.External;
using GymDAL.Entities;
using GymDAL.Repo.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymBLL.ModelVM.User.Member;
using GymBLL.ModelVM.Workout;
using GymBLL.ModelVM.Nutrition;
using GymDAL.Entities.Nutrition;

namespace GymBLL.Service.Implementation
{
    public class SubscriptionService : ISubscriptionService
    {
        public IMapper Mapper { get; }
        public IUnitOfWork UnitOfWork { get; }

        public SubscriptionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
        }

        public async Task<Response<SubscriptionVM>> CreateAsync(SubscriptionVM model)
        {
            try
            {
                var subscription = Mapper.Map<Subscription>(model);
                await UnitOfWork.Subscriptions.AddAsync(subscription);
                var result = await UnitOfWork.SaveAsync();
                
                if (result > 0)
                {
                    model.Id = subscription.Id;
                    return new Response<SubscriptionVM>(model, null, false);
                }
                return new Response<SubscriptionVM>(null, "Failed to create subscription.", true);
            }
            catch (Exception ex)
            {
                return new Response<SubscriptionVM>(null, $"Error: {ex.Message}", true);
            }
        }

        public async Task<Response<SubscriptionDetailsVM>> GetByIdAsync(int id)
        {
            try
            {
                var subscription = await UnitOfWork.Subscriptions.GetByIdAsync(id);
                if (subscription != null)
                {
                    var vm = Mapper.Map<SubscriptionDetailsVM>(subscription);
                    return new Response<SubscriptionDetailsVM>(vm, null, false);
                }
                return new Response<SubscriptionDetailsVM>(null, "Subscription not found.", true);
            }
            catch (Exception ex)
            {
                return new Response<SubscriptionDetailsVM>(null, $"Error: {ex.Message}", true);
            }
        }

        public async Task<Response<SubscriptionDetailsVM>> GetByMemeberIdAsync(string id)
        {
            try
            {
                var subscription = await UnitOfWork.Subscriptions.GetByIdAsync(id);
                if (subscription != null)
                {
                    var vm = Mapper.Map<SubscriptionDetailsVM>(subscription);
                    vm.MembershipVM=Mapper.Map<MembershipVM>(subscription.Membership);
                    vm.MemberVM=Mapper.Map<MemberVM>(subscription.Member);
                    vm.WorkoutAssignmentVM= Mapper.Map<WorkoutAssignmentVM>(subscription.WorkoutAssignment);
                    vm.DietPlanAssignmentVM= Mapper.Map<DietPlanAssignmentVM>(subscription.DietPlanAssignment);
                    if (subscription.DietPlanAssignment != null)
                    vm.DietPlanAssignmentVM.DietPlan= Mapper.Map<DietPlanVM>(subscription.DietPlanAssignment?.DietPlan);
                    if (subscription.WorkoutAssignment != null)
                    vm.WorkoutAssignmentVM.WorkoutPlan = Mapper.Map
                        <WorkoutPlanVM>(subscription.WorkoutAssignment?.WorkoutPlan);
                    vm.MemberVM.FitnessGoal.Id = subscription.Member.FitnessGoal.Id;
                    vm.MemberVM.FitnessGoal.GoalName = subscription.Member.FitnessGoal.GoalsName;
                    vm.MemberVM.FitnessGoal.GoalDescription = subscription.Member.FitnessGoal.GoalsDescription;



                    return new Response<SubscriptionDetailsVM>(vm, null, false);
                }
                return new Response<SubscriptionDetailsVM>(null, "Subscription not found.", true);
            }
            catch (Exception ex)
            {
                return new Response<SubscriptionDetailsVM>(null, $"Error: {ex.Message}", true);
            }

        }

        public async Task<Response<List<SubscriptionVM>>> GetAllAsync()
        {
            try
            {
                var subscriptions = await UnitOfWork.Subscriptions.GetAllAsync();
                var vms = subscriptions.Select(s => Mapper.Map<SubscriptionVM>(s)).ToList();
                return new Response<List<SubscriptionVM>>(vms, null, false);
            }
            catch (Exception ex)
            {
                return new Response<List<SubscriptionVM>>(null, $"Error: {ex.Message}", true);
            }
        }

        public async Task<Response<List<SubscriptionVM>>> GetByMemberIdAsync(string memberId)
        {
            try
            {
                var subscriptions = await UnitOfWork.Subscriptions.FindAsync(s => s.MemberId == memberId);
                var vms = subscriptions.Select(s => Mapper.Map<SubscriptionVM>(s)).ToList();
                return new Response<List<SubscriptionVM>>(vms, null, false);
            }
            catch (Exception ex)
            {
                return new Response<List<SubscriptionVM>>(null, $"Error: {ex.Message}", true);
            }
        }

        public async Task<Response<List<SubscriptionVM>>> GetActiveSubscriptionsAsync()
        {
            try
            {
                var subscriptions = await UnitOfWork.Subscriptions.FindAsync(s => s.Status.ToString() == "Active");
                var vms = subscriptions.Select(s => Mapper.Map<SubscriptionVM>(s)).ToList();
                return new Response<List<SubscriptionVM>>(vms, null, false);
            }
            catch (Exception ex)
            {
                return new Response<List<SubscriptionVM>>(null, $"Error: {ex.Message}", true);
            }
        }

        public async Task<Response<SubscriptionVM>> UpdateAsync(SubscriptionVM model)
        {
            try
            {
                var subscription = await UnitOfWork.Subscriptions.GetByIdAsync(model.Id);
                if (subscription == null)
                    return new Response<SubscriptionVM>(null, "Subscription not found.", true);

                subscription.MembershipId = model.MembershipId;
                subscription.StartDate = model.StartDate;
                subscription.EndDate = model.EndDate;
                subscription.Status = Enum.Parse<SubscriptionStatus>(model.Status);
              

                UnitOfWork.Subscriptions.Update(subscription);
                var result = await UnitOfWork.SaveAsync();
                
                if (result > 0)
                    return new Response<SubscriptionVM>(model, null, false);
                return new Response<SubscriptionVM>(null, "Failed to update subscription.", true);
            }
            catch (Exception ex)
            {
                return new Response<SubscriptionVM>(null, $"Error: {ex.Message}", true);
            }
        }
        public async Task<Response<SubscriptionDetailsVM>> UpdateAsync(SubscriptionDetailsVM model)
        {
            try
            {
                var subscription = await UnitOfWork.Subscriptions.GetByIdAsync(model.Id);
                if (subscription == null)
                    return new Response<SubscriptionDetailsVM>(null, "Subscription not found.", true);

                subscription.MembershipId = model.MembershipId;
                subscription.StartDate = model.StartDate;
                subscription.EndDate = model.EndDate;
                subscription.Status = Enum.Parse<SubscriptionStatus>(model.Status);
                subscription.DietPlanAssignmentId = model.DietPlanAssignmentVM?.Id;
                subscription.WorkoutAssignmentId = model.WorkoutAssignmentVM?.Id;

             
                UnitOfWork.Subscriptions.Update(subscription);
                var result = await UnitOfWork.SaveAsync();

                if (result > 0)
                    return new Response<SubscriptionDetailsVM>(model, null, false);
                return new Response<SubscriptionDetailsVM>(null, "Failed to update subscription.", true);
            }
            catch (Exception ex)
            {
                return new Response<SubscriptionDetailsVM>(null, $"Error: {ex.Message}", true);
            }
        }

        public async Task<Response<bool>> CancelSubscriptionAsync(int id)
        {
            try
            {
                var subscription = await UnitOfWork.Subscriptions.GetByIdAsync(id);
                if (subscription == null)
                    return new Response<bool>(false, "Subscription not found.", true);

                subscription.Status = SubscriptionStatus.Cancelled;
                UnitOfWork.Subscriptions.Update(subscription);
                var result = await UnitOfWork.SaveAsync();
                
                if (result > 0)
                    return new Response<bool>(true, null, false);
                return new Response<bool>(false, "Failed to cancel subscription.", true);
            }
            catch (Exception ex)
            {
                return new Response<bool>(false, $"Error: {ex.Message}", true);
            }
        }

        public async Task<Response<bool>> RenewSubscriptionAsync(int id)
        {
            try
            {
                var subscription = await UnitOfWork.Subscriptions.GetByIdAsync(id);
                if (subscription == null)
                    return new Response<bool>(false, "Subscription not found.", true);

                var membership = await UnitOfWork.Memberships.GetByIdAsync(subscription.MembershipId);
                if (membership != null)
                {
                    subscription.EndDate = subscription.EndDate.AddMonths(membership.DurationInMonths);
                    subscription.Status = SubscriptionStatus.Active;
                    UnitOfWork.Subscriptions.Update(subscription);
                    var result = await UnitOfWork.SaveAsync();
                    
                    if (result > 0)
                        return new Response<bool>(true, null, false);
                }
                return new Response<bool>(false, "Failed to renew subscription.", true);
            }
            catch (Exception ex)
            {
                return new Response<bool>(false, $"Error: {ex.Message}", true);
            }
        }

        public async Task<Response<bool>> DeleteAsync(int id)
        {
            try
            {
                var subscription = await UnitOfWork.Subscriptions.GetByIdAsync(id);
                if (subscription == null)
                    return new Response<bool>(false, "Subscription not found.", true);

                UnitOfWork.Subscriptions.Remove(subscription);
                var result = await UnitOfWork.SaveAsync();
                
                if (result > 0)
                    return new Response<bool>(true, null, false);
                return new Response<bool>(false, "Failed to delete subscription.", true);
            }
            catch (Exception ex)
            {
                return new Response<bool>(false, $"Error: {ex.Message}", true);
            }
        }

        public async Task<Response<List<SubscriptionVM>>> GetExpiringSubscriptionsAsync(int daysUntilExpiry)
        {
            try
            {
                var targetDate = DateTime.UtcNow.AddDays(daysUntilExpiry);
                var subscriptions = await UnitOfWork.Subscriptions.FindAsync(s => 
                    s.Status == SubscriptionStatus.Active && 
                    s.EndDate <= targetDate && 
                    s.EndDate > DateTime.UtcNow);
                
                var vms = subscriptions.Select(s => Mapper.Map<SubscriptionVM>(s)).ToList();
                return new Response<List<SubscriptionVM>>(vms, null, false);
            }
            catch (Exception ex)
            {
                return new Response<List<SubscriptionVM>>(null, $"Error: {ex.Message}", true);
            }
        }

        public async Task<Response<List<SubscriptionVM>>> GetExpiredSubscriptionsAsync()
        {
            try
            {
                var subscriptions = await UnitOfWork.Subscriptions.FindAsync(s => 
                    s.Status == SubscriptionStatus.Active && 
                    s.EndDate < DateTime.UtcNow);
                
                var vms = subscriptions.Select(s => Mapper.Map<SubscriptionVM>(s)).ToList();
                return new Response<List<SubscriptionVM>>(vms, null, false);
            }
            catch (Exception ex)
            {
                return new Response<List<SubscriptionVM>>(null, $"Error: {ex.Message}", true);
            }
        }

        public async Task<Response<SubscriptionVM>> GetActiveSubscriptionByMemberIdAsync(string memberId)
        {
            try
            {
                var subscriptions = await UnitOfWork.Subscriptions.FindAsync(s => 
                    s.MemberId == memberId && 
                    s.Status == SubscriptionStatus.Active);
                
                var activeSubscription = subscriptions.OrderByDescending(s => s.EndDate).FirstOrDefault();
                
                if (activeSubscription != null)
                {
                    var vm = Mapper.Map<SubscriptionVM>(activeSubscription);
                    return new Response<SubscriptionVM>(vm, null, false);
                }
                
                return new Response<SubscriptionVM>(null, "No active subscription found.", true);
            }
            catch (Exception ex)
            {
                return new Response<SubscriptionVM>(null, $"Error: {ex.Message}", true);
            }
        }

        public async Task<Response<SubscriptionVM>> UpgradeSubscriptionAsync(int currentSubscriptionId, int newMembershipId, PaymentVM payment)
        {
            try
            {
                // Get current subscription
                var currentSubscription = await UnitOfWork.Subscriptions.GetByIdAsync(currentSubscriptionId);
                if (currentSubscription == null)
                    return new Response<SubscriptionVM>(null, "Current subscription not found.", true);

                // Get new membership
                var newMembership = await UnitOfWork.Memberships.GetByIdAsync(newMembershipId);
                if (newMembership == null)
                    return new Response<SubscriptionVM>(null, "New membership plan not found.", true);

                // Cancel current subscription
                currentSubscription.Status = SubscriptionStatus.Cancelled;
                UnitOfWork.Subscriptions.Update(currentSubscription);

                // Create payment record
                var paymentEntity = Mapper.Map<Payment>(payment);
                paymentEntity.Status = "Completed";
                paymentEntity.ProcessedDate = DateTime.UtcNow;
                await UnitOfWork.Payments.AddAsync(paymentEntity);
                await UnitOfWork.SaveAsync();

                // Create new subscription
                var newSubscription = new Subscription
                {
                    MemberId = currentSubscription.MemberId,
                    MembershipId = newMembershipId,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddMonths(newMembership.DurationInMonths),
                    Status = SubscriptionStatus.Active,
                    PaymentId = paymentEntity.Id,
                    CreatedAt = DateTime.UtcNow
                };

                await UnitOfWork.Subscriptions.AddAsync(newSubscription);
                var result = await UnitOfWork.SaveAsync();

                if (result > 0)
                {
                    var vm = Mapper.Map<SubscriptionVM>(newSubscription);
                    return new Response<SubscriptionVM>(vm, null, false);
                }

                return new Response<SubscriptionVM>(null, "Failed to upgrade subscription.", true);
            }
            catch (Exception ex)
            {
                return new Response<SubscriptionVM>(null, $"Error: {ex.Message}", true);
            }
        }

        public async Task<Response<SubscriptionVM>> GetMembershipAsync(int id)
        {
            try
            {
                var subscription = await UnitOfWork.Subscriptions.GetByIdAsync(id);
                if (subscription != null)
                {
                    var vm = Mapper.Map<SubscriptionVM>(subscription);
                    return new Response<SubscriptionVM>(vm, null, false);
                }
                return new Response<SubscriptionVM>(null, "Subscription not found.", true);
            }
            catch (Exception ex)
            {
                return new Response<SubscriptionVM>(null, $"Error: {ex.Message}", true);
            }
        }

        public async Task<Response<(bool hasWorkout, bool hasDiet)>> CheckActivePlanAsync(string memberId)
        {
            try
            {
                var result = await UnitOfWork.Subscriptions.GetByIdAsync(memberId);

                if (result == null || result.Membership == null)
                {
                    return new Response<(bool, bool)>((false, false), "Member or membership not found", true);
                }

                var hasDiet = result.DietPlanAssignmentId>0
                    ;
                var hasWorkout = result.WorkoutAssignmentId>0;

                return new Response<(bool hasWorkout, bool hasDiet)>((hasWorkout, hasDiet), null, false);
            }
            catch (Exception ex)
            {
                return new Response<(bool hasWorkout, bool hasDiet)>((false, false), $"Error: {ex.Message}", true);
            }
        }
    }
}
