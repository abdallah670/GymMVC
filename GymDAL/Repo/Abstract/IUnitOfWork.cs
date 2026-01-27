

using GymDAL.Repo.Abstract.Financial;
using GymDAL.Repo.Abstract.Core;
using GymDAL.Repo.Abstract.Communication;
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
            ITempRegistrationRepository TempRegistrationRepository { get; }
            ITrainerReviewRepository TrainerReviews { get; }

            // Plan-related
            IWorkoutPlanRepository WorkoutPlans { get; }
            IDietPlanRepository DietPlans { get; }
            IDietPlanItemRepository DietPlanItems { get; }

        // Assignment-related
        IWorkoutAssignmentRepository WorkoutAssignments { get; }
            IDietPlanAssignmentRepository DietPlanAssignments { get; }
             IWorkoutPlanItemRepository WorkoutPlanItems { get; }
             IWorkoutLogRepository WorkoutLogs { get; }
        // Payment-related
        IPaymentRepository Payments { get; }
            IMembershipRepository Memberships { get; }


        // Log-related

        IMealLogRepository MealLogs { get; }
            ISubscriptionRepository Subscriptions { get; }
        IFitnessGoalsRepository FitnessGoalsRepository { get; }

        // System-related
        INotificationRepository Notifications { get; }
        IChatMessageRepository ChatMessages { get; }
        IWeightLogRepository WeightLogs { get; }

        Task<int> SaveAsync();
            void BeginTransaction();
            Task CommitTransactionAsync();
            void RollbackTransaction();
        }
}