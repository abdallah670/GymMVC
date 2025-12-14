using AutoMapper;
using GymDAL.Entities.External;
using GymDAL.Repo.Abstract.Workout;

namespace GymDAL.Repo.Implementation
{
    public class WorkoutPlanRepository : Repository<WorkoutPlan>, IWorkoutPlanRepository
    {
        public WorkoutPlanRepository(GymDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public Task<string> GetWorkoutPlanNameAsync(int Id)
        {
            try
            {

                var workoutPlan = _context.WorkoutPlans.Select(
                    wp => new WorkoutPlan
                    {
                        Id = wp.Id,
                        Name = wp.Name
                    }

                    ).FirstOrDefault(wp => wp.Id == Id);
                if (workoutPlan != null)
                {
                    return Task.FromResult(workoutPlan.Name);
                }
                return Task.FromResult(string.Empty);
            }

            catch (Exception ex)
            {
                // Log the exception (logging mechanism not shown here)
                throw new Exception("An error occurred while retrieving the workout plan name.", ex);
            }
        }
       
        public override async Task<WorkoutPlan> GetByIdAsync(int id)
        {
            try
            {
                return await _context.WorkoutPlans.Include(w => w.WorkoutPlanItems).
                    FirstOrDefaultAsync(w => w.Id == id);
            }
            catch
            {
                throw;
            }

        }
    }
}