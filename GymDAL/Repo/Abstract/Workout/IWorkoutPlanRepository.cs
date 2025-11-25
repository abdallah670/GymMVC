namespace GymDAL.Repo.Abstract.Workout
{
    public interface IWorkoutPlanRepository : IRepository<WorkoutPlan>
    {
       
        // GET Operations
        Task<WorkoutPlan> GetByAsync(Expression<Func<WorkoutPlan, bool>>? Filter);
        Task<IEnumerable<WorkoutPlan>> GetAsync(Expression<Func<WorkoutPlan, bool>>? Filter);
        // CRWorkoutPlanperations
        Task<WorkoutPlan> CreateAsync(WorkoutPlan Plan, string Createdby);
        // UPWorkoutPlanperations
        Task<WorkoutPlan> UpdateAsync(WorkoutPlan user, string UpdatedBy);
        // DELETE Operations
        Task<bool> ToggleStatusAsync(int planId, string DeletedBy);
    }
}