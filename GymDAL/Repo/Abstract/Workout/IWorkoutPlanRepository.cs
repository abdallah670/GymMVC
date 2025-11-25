namespace GymDAL.Repo.Abstract.Workout
{
    public interface IWorkoutPlanRepository : IRepository<WorkoutPlan>
    {
        Task<string> GetWorkoutPlanNameAsync(int Id);


    }
}