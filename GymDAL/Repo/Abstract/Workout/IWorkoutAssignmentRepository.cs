using System.Linq.Expressions;

namespace GymDAL.Repo.Abstract.Workout
{
    public interface IWorkoutAssignmentRepository : IRepository<WorkoutAssignment>
    {
        // GET Operations
        Task<WorkoutAssignment> GetByAsync(Expression<Func<WorkoutAssignment, bool>>? Filter);
        Task<IEnumerable<WorkoutAssignment>> GetAsync(Expression<Func<WorkoutAssignment, bool>>? Filter);

        // CREATE Operations
        Task<WorkoutAssignment> CreateAsync(WorkoutAssignment assignment, string Createdby);

        // UPDATE Operations
        Task<WorkoutAssignment> UpdateAsync(WorkoutAssignment assignment, string UpdatedBy);

        // DELETE Operations
        Task<bool> ToggleStatusAsync(int assignmentId, string DeletedBy);
    }
}