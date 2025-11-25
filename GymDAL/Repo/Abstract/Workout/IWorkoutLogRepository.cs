using System.Linq.Expressions;

namespace GymDAL.Repo.Abstract.Workout
{
    public interface IWorkoutLogRepository : IRepository<WorkoutLog>
    {
        // GET Operations
        Task<WorkoutLog> GetByAsync(Expression<Func<WorkoutLog, bool>>? Filter);
        Task<IEnumerable<WorkoutLog>> GetAsync(Expression<Func<WorkoutLog, bool>>? Filter);

        // CREATE Operations
        Task<WorkoutLog> CreateAsync(WorkoutLog log, string Createdby);

        // UPDATE Operations
        Task<WorkoutLog> UpdateAsync(WorkoutLog log, string UpdatedBy);

        // DELETE Operations
        Task<bool> ToggleStatusAsync(int logId, string DeletedBy);
    }
}