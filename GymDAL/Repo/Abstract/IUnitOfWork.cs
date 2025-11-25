

using GymDAL.Interfaces.External;
using GymDAL.Repo.Abstract.Extra;

using GymDAL.Repo.Abstract.Nutrition;
using GymDAL.Repo.Abstract.Users;
using GymDAL.Repo.Abstract.Workout;

namespace GymDAL.Repo.Abstract
{
    public interface IUnitOfWork : IDisposable
    {



        // User-related
            IApplicationUserRepository ApplicationUsers { get; }
          
            IMemberRepository Members { get; }
            ITrainerRepository Trainers { get; }

            // Plan-related
            IWorkoutPlanRepository WorkoutPlans { get; }
            IDietPlanRepository DietPlans { get; }
            IDietPlanItemRepository DietPlanItems { get; }

        // Assignment-related
        IWorkoutAssignmentRepository WorkoutAssignments { get; }
            IDietPlanAssignmentRepository DietPlanAssignments { get; }
             IWorkoutPlanItemRepository WorkoutPlanItems { get; }
        // Payment-related
        IPaymentRepository Payments { get; }
            IMembershipRepository Memberships { get; }


        // Log-related

        IMealLogRepository MealLogs { get; }
            ISubscriptionRepository Subscriptions { get; }
        IFitnessGoalsRepository FitnessGoalsRepository { get; }

        // System-related
        INotificationRepository Notifications { get; }
          

            Task<int> SaveAsync();
            void BeginTransaction();
            Task CommitTransactionAsync();
            void RollbackTransaction();
        }
}