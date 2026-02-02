using GymBLL.Response;
using GymBLL.ModelVM;
using GymBLL.ModelVM.Financial;
using GymBLL.Service.Abstract.Financial;
using AutoMapper;
using GymBLL.Service.Abstract;
using GymDAL.Enums;
using GymDAL.Entities.Financial;
using GymDAL.Entities;
using GymDAL.Repo.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymBLL.ModelVM.Member;
using GymBLL.ModelVM.Workout;
using GymBLL.ModelVM.Nutrition;
using GymDAL.Entities.Nutrition;
namespace GymBLL.Service.Implementation.Financial
{
    public class SubscriptionService : ISubscriptionService
    {
        public IMapper Mapper
        {
            get;
        }
        public IUnitOfWork UnitOfWork
        {
            get;
        }
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
                    vm.MembershipVM = Mapper.Map<MembershipVM>(subscription.Membership);
                    vm.MemberVM = Mapper.Map<MemberVM>(subscription.Member);
                    vm.WorkoutAssignmentVM = Mapper.Map<WorkoutAssignmentVM>(subscription.WorkoutAssignment);
                    vm.DietPlanAssignmentVM = Mapper.Map<DietPlanAssignmentVM>(subscription.DietPlanAssignment);
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
                // Find subscription by MemberId, not GetByIdAsync which uses primary key
                // Usage example
                var subscriptions = await UnitOfWork.Subscriptions
                    .Get(
                        filter: s => s.MemberId == id && s.Status == GymDAL.Enums.SubscriptionStatus.Active && s.EndDate >= DateTime.UtcNow,
                        include: query => query
                            .Include(s => s.WorkoutAssignment)
                                .ThenInclude(wa => wa.WorkoutPlan)
                            .Include(s => s.DietPlanAssignment)
                                .ThenInclude(dpa => dpa.DietPlan)
                            .Include(s => s.Member).ThenInclude(m=>m.FitnessGoal)
                            .Include(s => s.Membership)
                    )
                    .ToListAsync();
                var subscription = subscriptions.OrderByDescending(s => s.EndDate).FirstOrDefault();
                
                if (subscription == null)
                {
                    // Return null result but NOT an error - member simply has no subscription
                    return new Response<SubscriptionDetailsVM>(null, null, false);
                }
                
                var vm = Mapper.Map<SubscriptionDetailsVM>(subscription);
                vm.MembershipVM = subscription.Membership != null ? Mapper.Map<MembershipVM>(subscription.Membership) : null;
                vm.MemberVM = subscription.Member != null ? Mapper.Map<MemberVM>(subscription.Member) : null;
                vm.WorkoutAssignmentVM = subscription.WorkoutAssignment != null ?
                    Mapper.Map<WorkoutAssignmentVM>(subscription.WorkoutAssignment) : null;
                vm.DietPlanAssignmentVM = subscription.DietPlanAssignment != null ? 
                    Mapper.Map<DietPlanAssignmentVM>(subscription.DietPlanAssignment) : null;
                
                if (subscription.DietPlanAssignment != null && vm.DietPlanAssignmentVM != null) 
                    vm.DietPlanAssignmentVM.DietPlan = Mapper.Map<DietPlanVM>(subscription.DietPlanAssignment.DietPlan);
                if (subscription.WorkoutAssignment != null && vm.WorkoutAssignmentVM != null) 
                    vm.WorkoutAssignmentVM.WorkoutPlan = Mapper.Map<WorkoutPlanVM>(subscription.WorkoutAssignment.WorkoutPlan);
                    
                if (subscription.Member?.FitnessGoal != null && vm.MemberVM != null)
                {
                    if (vm.MemberVM.FitnessGoal == null) vm.MemberVM.FitnessGoal = new GymBLL.ModelVM.Member.FitnessGoalsVM();
                    vm.MemberVM.FitnessGoal.Id = subscription.Member.FitnessGoal.Id;
                    vm.MemberVM.FitnessGoal.GoalName = subscription.Member.FitnessGoal.GoalName;
                    vm.MemberVM.FitnessGoal.GoalDescription = subscription.Member.FitnessGoal.GoalDescription;
                }
                return new Response<SubscriptionDetailsVM>(vm, null, false);
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

                // Include related entities to get member and membership data
                var subscriptions = await UnitOfWork.Subscriptions.GetAllAsync();


                var vms = subscriptions.Select(s => new SubscriptionVM
                {
                    Id = s.Id,
                    MemberId = s.MemberId,
                    MemberName = s.Member?.FullName ?? s.Member?.UserName,
                    // Adjust based on your Member entity
                    MembershipId = s.MembershipId,
                    MembershipType = s.Membership?.MembershipType ?? s.Membership?.MembershipType,
                    // Adjust based on your Membership entity
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    Status = s.Status.ToString(),
                    PaymentId = s.PaymentId,
                    CreatedAt = s.CreatedAt
                }).ToList();


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
                if (subscription == null) return new Response<SubscriptionVM>(null, "Subscription not found.", true);
                subscription.MembershipId = model.MembershipId;
                subscription.StartDate = model.StartDate;
                subscription.EndDate = model.EndDate;
                subscription.Status = Enum.Parse<SubscriptionStatus>(model.Status);
                // Update assignment IDs if provided
                if (model.WorkoutAssignmentId.HasValue)
                    subscription.WorkoutAssignmentId = model.WorkoutAssignmentId;
                if (model.DietPlanAssignmentId.HasValue)
                    subscription.DietPlanAssignmentId = model.DietPlanAssignmentId;
                UnitOfWork.Subscriptions.Update(subscription);
                var result = await UnitOfWork.SaveAsync();
                if (result > 0) return new Response<SubscriptionVM>(model, null, false);
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
                if (subscription == null) return new Response<SubscriptionDetailsVM>(null, "Subscription not found.", true);
                subscription.MembershipId = model.MembershipId;
                subscription.StartDate = model.StartDate;
                subscription.EndDate = model.EndDate;
                subscription.Status = Enum.Parse<SubscriptionStatus>(model.Status);
                subscription.DietPlanAssignmentId = model.DietPlanAssignmentVM?.Id;
                subscription.WorkoutAssignmentId = model.WorkoutAssignmentVM?.Id;
                UnitOfWork.Subscriptions.Update(subscription);
                var result = await UnitOfWork.SaveAsync();
                if (result > 0) return new Response<SubscriptionDetailsVM>(model, null, false);
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
                if (subscription == null) return new Response<bool>(false, "Subscription not found.", true);
                subscription.Status = SubscriptionStatus.Cancelled;
                UnitOfWork.Subscriptions.Update(subscription);
                var result = await UnitOfWork.SaveAsync();
                if (result > 0) return new Response<bool>(true, null, false);
                return new Response<bool>(false, "Failed to cancel subscription.", true);
            }
            catch (Exception ex)
            {
                return new Response<bool>(false, $"Error: {ex.Message}", true);
            }
        }
        public async Task<Response<bool>> RenewSubscriptionAsync(int id, PaymentVM? payment = null)
        {
            try
            {
                UnitOfWork.BeginTransaction();

                var subscription = await UnitOfWork.Subscriptions.GetByIdAsync(id);
                if (subscription == null)
                {
                    UnitOfWork.RollbackTransaction();
                    return new Response<bool>(false, "Subscription not found.", true);
                }

                var membership = await UnitOfWork.Memberships.GetByIdAsync(subscription.MembershipId);
                if (membership == null)
                {
                    UnitOfWork.RollbackTransaction();
                    return new Response<bool>(false, "Membership plan not found for this subscription.", true);
                }

                // If renewing, record the payment if provided
                if (payment != null)
                {
                    var paymentEntity = Mapper.Map<Payment>(payment);
                    paymentEntity.Status = "Completed";
                    paymentEntity.ProcessedDate = DateTime.UtcNow;
                    await UnitOfWork.Payments.AddAsync(paymentEntity);
                    await UnitOfWork.SaveAsync();
                    subscription.PaymentId = paymentEntity.Id;
                }

                // IMPORTANT: Calculate EndDate properly: 
                // If current subscription is still active (EndDate > Now), add months to current EndDate.
                // If it's already expired, add months to Now.
                var baseDate = subscription.EndDate > DateTime.UtcNow ? subscription.EndDate : DateTime.UtcNow;
                subscription.EndDate = baseDate.AddMonths(membership.DurationInMonths);
                subscription.Status = SubscriptionStatus.Active;

                UnitOfWork.Subscriptions.Update(subscription);
                var result = await UnitOfWork.SaveAsync();
                
                if (result > 0)
                {
                    await UnitOfWork.CommitTransactionAsync();
                    return new Response<bool>(true, null, false);
                }

                UnitOfWork.RollbackTransaction();
                return new Response<bool>(false, "Failed to update subscription data.", true);
            }
            catch (Exception ex)
            {
                UnitOfWork.RollbackTransaction();
                return new Response<bool>(false, $"Error during renewal: {ex.Message}", true);
            }
        }
        public async Task<Response<bool>> DeleteAsync(int id)
        {
            try
            {
                var subscription = await UnitOfWork.Subscriptions.GetByIdAsync(id);
                if (subscription == null) return new Response<bool>(false, "Subscription not found.", true);
                UnitOfWork.Subscriptions.Remove(subscription);
                var result = await UnitOfWork.SaveAsync();
                if (result > 0) return new Response<bool>(true, null, false);
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
                var subscriptions = await UnitOfWork.Subscriptions.FindAsync(s => s.Status == SubscriptionStatus.Active && s.EndDate <= targetDate && s.EndDate > DateTime.UtcNow);
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
                var subscriptions = await UnitOfWork.Subscriptions.FindAsync(s => s.Status == SubscriptionStatus.Active && s.EndDate < DateTime.UtcNow);
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
                // Filter for Active status and valid EndDate to match GetByMemeberIdAsync behavior
                var subscriptions = await UnitOfWork.Subscriptions.FindAsync(
                    s => s.MemberId == memberId && 
                         s.Status == GymDAL.Enums.SubscriptionStatus.Active && 
                         s.EndDate >= DateTime.UtcNow);
                var activeSubscription = subscriptions.OrderByDescending(s => s.EndDate).FirstOrDefault();
                if (activeSubscription != null)
                {
                    var vm = Mapper.Map<SubscriptionVM>(activeSubscription);
                    // Ensure assignment IDs are mapped (they should be by convention, but be explicit)
                    vm.WorkoutAssignmentId = activeSubscription.WorkoutAssignmentId;
                    vm.DietPlanAssignmentId = activeSubscription.DietPlanAssignmentId;
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
                UnitOfWork.BeginTransaction();

                // Verify new membership exists
                var newMembership = await UnitOfWork.Memberships.GetByIdAsync(newMembershipId);
                if (newMembership == null) 
                {
                    UnitOfWork.RollbackTransaction();
                    return new Response<SubscriptionVM>(null, "New membership plan not found.", true);
                }

                // Verify current subscription exists
                var currentSubscription = await UnitOfWork.Subscriptions.GetByIdAsync(currentSubscriptionId);
                if (currentSubscription == null)
                {
                    UnitOfWork.RollbackTransaction();
                    return new Response<SubscriptionVM>(null, "Current subscription not found.", true);
                }

                // Create payment record
                var paymentEntity = Mapper.Map<Payment>(payment);
                paymentEntity.Status = "Completed";
                paymentEntity.ProcessedDate = DateTime.UtcNow;
                
                await UnitOfWork.Payments.AddAsync(paymentEntity);
                await UnitOfWork.SaveAsync();

                // Create new subscription and TRANSFER EXISTING ASSIGNMENTS
                var newSubscription = new Subscription
                {
                    MemberId = currentSubscription.MemberId,
                    MembershipId = newMembershipId,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddMonths(newMembership.DurationInMonths),
                    Status = SubscriptionStatus.Active,
                    PaymentId = paymentEntity.Id,
                    CreatedAt = DateTime.UtcNow,
                    // Transfer assignments so the user doesn't lose their data
                    WorkoutAssignmentId = currentSubscription.WorkoutAssignmentId,
                    DietPlanAssignmentId = currentSubscription.DietPlanAssignmentId
                };

                // Cancel current subscription
                currentSubscription.Status = SubscriptionStatus.Cancelled;
                UnitOfWork.Subscriptions.Update(currentSubscription);

                await UnitOfWork.Subscriptions.AddAsync(newSubscription);
                var result = await UnitOfWork.SaveAsync();

                if (result > 0)
                {
                    await UnitOfWork.CommitTransactionAsync();
                    var vm = Mapper.Map<SubscriptionVM>(newSubscription);
                    return new Response<SubscriptionVM>(vm, null, false);
                }
                
                UnitOfWork.RollbackTransaction();
                return new Response<SubscriptionVM>(null, "Failed to upgrade subscription data.", true);
            }
            catch (Exception ex)
            {
                UnitOfWork.RollbackTransaction();
                return new Response<SubscriptionVM>(null, $"Error during upgrade: {ex.Message}", true);
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
        public async Task<Response<(bool hasWorkout, int? workoutAssignmentId, bool hasDiet, int? dietPlanAssignmentId)>> CheckActivePlanAsync(string memberId)
        {
            try
            {
                // Find subscription by MemberId with consistent filtering (matching GetByMemeberIdAsync)
                var subscriptions = await UnitOfWork.Subscriptions.FindAsync(
                    s => s.MemberId == memberId && 
                         s.Status == GymDAL.Enums.SubscriptionStatus.Active && 
                         s.EndDate >= DateTime.UtcNow);
                var result = subscriptions.OrderByDescending(s => s.EndDate).FirstOrDefault();
                
                if (result == null)
                {
                    // No subscription is NOT an error - just return false for both
                    return new Response<(bool, int?, bool, int?)>((false, null, false, null), null, false);
                }
                
                var hasDiet = result.DietPlanAssignmentId > 0;
                var hasWorkout = result.WorkoutAssignmentId > 0;
                return new Response<(bool hasWorkout, int? workoutAssignmentId, bool hasDiet, int? dietPlanAssignmentId)>((hasWorkout, result.WorkoutAssignmentId, hasDiet, result.DietPlanAssignmentId), null, false);
            }
            catch (Exception ex)
            {
                return new Response<(bool hasWorkout, int? workoutAssignmentId, bool hasDiet, int? dietPlanAssignmentId)>((false, null, false, null), $"Error: {ex.Message}", true);
            }
        }

        public async Task<Response<SubscriptionDetailsVM>> GetSubscritionDietByMemberIdAsync(string memberId)
        {
            try
            {
                // Find subscription by MemberId, not GetByIdAsync which uses primary key
                // Usage example
                var subscriptions = await UnitOfWork.Subscriptions
                    .Get(
                        filter: s => s.MemberId == memberId && s.Status == GymDAL.Enums.SubscriptionStatus.Active && s.EndDate >= DateTime.UtcNow,
                        include: query => query.Include(s => s.DietPlanAssignment)
                                .ThenInclude(dpa => dpa.DietPlan).ThenInclude(d => d.DietPlanItems)
                    )
                    .ToListAsync();
                var subscription = subscriptions.OrderByDescending(s => s.EndDate).FirstOrDefault();

                if (subscription == null)
                {
                    // Return null result but NOT an error - member simply has no subscription
                    return new Response<SubscriptionDetailsVM>(null, null, false);
                }

                var vm = Mapper.Map<SubscriptionDetailsVM>(subscription);
              
               
                vm.DietPlanAssignmentVM = subscription.DietPlanAssignment != null ?
                    Mapper.Map<DietPlanAssignmentVM>(subscription.DietPlanAssignment) : null;

                if (subscription.DietPlanAssignment != null && vm.DietPlanAssignmentVM != null)
                    vm.DietPlanAssignmentVM.DietPlan = Mapper.Map<DietPlanVM>(subscription.DietPlanAssignment.DietPlan);
              
                return new Response<SubscriptionDetailsVM>(vm, null, false);
            }
            catch (Exception ex)
            {
                return new Response<SubscriptionDetailsVM>(null, $"Error: {ex.Message}", true);
            }
        }

        public async Task<Response<SubscriptionDetailsVM>> GetSubscritionWorkoutByMemberIdAsync(string memberId)
        {
            try
            {
                // Find subscription by MemberId, not GetByIdAsync which uses primary key
                // Usage example
                var subscriptions = await UnitOfWork.Subscriptions
                    .Get(
                        filter: s => s.MemberId == memberId && s.Status == GymDAL.Enums.SubscriptionStatus.Active && s.EndDate >= DateTime.UtcNow,
                        include: query => query.AsNoTracking()
                            .Include(s => s.WorkoutAssignment)
                                .ThenInclude(wa => wa.WorkoutPlan).ThenInclude(w=>w.WorkoutPlanItems)
                           
                    )
                    .ToListAsync();
                var subscription = subscriptions.OrderByDescending(s => s.EndDate).FirstOrDefault();

                if (subscription == null)
                {
                    // Return null result but NOT an error - member simply has no subscription
                    return new Response<SubscriptionDetailsVM>(null, null, false);
                }

                var vm = Mapper.Map<SubscriptionDetailsVM>(subscription);
               
                vm.WorkoutAssignmentVM = subscription.WorkoutAssignment != null ?
                    Mapper.Map<WorkoutAssignmentVM>(subscription.WorkoutAssignment) : null;
              
                if (subscription.WorkoutAssignment != null && vm.WorkoutAssignmentVM != null)
                    vm.WorkoutAssignmentVM.WorkoutPlan = Mapper.Map<WorkoutPlanVM>(subscription.WorkoutAssignment.WorkoutPlan);
                return new Response<SubscriptionDetailsVM>(vm, null, false);
            }
            catch (Exception ex)
            {
                return new Response<SubscriptionDetailsVM>(null, $"Error: {ex.Message}", true);
            }
        }

        public async Task<Response<int>> GetAssignedDietPlan(string MemberId)
        {
            try
            {
                // Usage example
                var subscription = await UnitOfWork.Subscriptions
                    .Get(
                        filter: s => s.MemberId == MemberId && s.Status == GymDAL.Enums.SubscriptionStatus.Active
                    ).AsNoTracking().FirstOrDefaultAsync();
               
                if (subscription == null || !subscription.DietPlanAssignmentId.HasValue)
                {
                    return new Response<int>(0, null, false);
                }

                return new Response<int>(subscription.DietPlanAssignmentId.Value, null, false);

            }
            catch (Exception ex) {
                return new Response<int>(0, $"Error: {ex.Message}", true);
            }
        }
    }
}



