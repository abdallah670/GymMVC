

using GymDAL.Repo.Abstract.Extra;
using GymDAL.Repo.Abstract.Logs;
using GymDAL.Repo.Abstract.Nutrition;
using GymDAL.Repo.Abstract.Users;
using GymDAL.Repo.Abstract.Workout;

namespace GymDAL.Repo.Abstract
{
    public interface IUnitOfWork : IDisposable
    {



            // User-related
            IMemberRepository Members { get; }
            ITrainerRepository Trainers { get; }

            // Plan-related
            IWorkoutPlanRepository WorkoutPlans { get; }
            IDietPlanRepository DietPlans { get; }

            // Assignment-related
            IWorkoutAssignmentRepository WorkoutAssignments { get; }
            IDietPlanAssignmentRepository DietPlanAssignments { get; }

            // Log-related
            IWorkoutLogRepository WorkoutLogs { get; }
            IMealLogRepository MealLogs { get; }
            IProgressLogRepository ProgressLogs { get; }

            // System-related
            INotificationRepository Notifications { get; }
            IReportLogRepository ReportLogs { get; }

            Task<int> SaveAsync();
            void BeginTransaction();
            Task CommitTransactionAsync();
            void RollbackTransaction();
        }
}