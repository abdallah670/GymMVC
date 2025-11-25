using System.Linq.Expressions;

namespace GymDAL.Repo.Abstract.Workout
{
    public interface IWorkoutPlanItemRepository : IRepository<WorkoutPlanItem>
    {
        // GET Operations
        Task<WorkoutPlanItem> GetByAsync(Expression<Func<WorkoutPlanItem, bool>>? Filter);
        Task<IEnumerable<WorkoutPlanItem>> GetAsync(Expression<Func<WorkoutPlanItem, bool>>? Filter);

        // CREATE Operations
        Task<WorkoutPlanItem> CreateAsync(WorkoutPlanItem item, string Createdby);

        // UPDATE Operations
        Task<WorkoutPlanItem> UpdateAsync(WorkoutPlanItem item, string UpdatedBy);

        // DELETE Operations
        Task<bool> ToggleStatusAsync(int itemId, string DeletedBy);
    }
}