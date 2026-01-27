using System.Linq.Expressions;

namespace GymDAL.Repo.Abstract.Workout
{
    public interface IWorkoutAssignmentRepository : IRepository<WorkoutAssignment>
    {
        Task<WorkoutAssignment> GetDetailed(int ?WorkoutasssignmentId);
    }
}