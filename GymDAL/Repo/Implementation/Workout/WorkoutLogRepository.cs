using GymDAL.DB;
using GymDAL.Entities.Workout;
using GymDAL.Repo.Abstract.Workout;

namespace GymDAL.Repo.Implementation.Workout
{
    public class WorkoutLogRepository : Repository<WorkoutLog>, IWorkoutLogRepository
    {
        public WorkoutLogRepository(GymDbContext context) : base(context)
        {
        }
    }
}
