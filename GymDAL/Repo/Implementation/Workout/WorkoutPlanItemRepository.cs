using AutoMapper;
using GymDAL.Repo.Abstract.Workout;

namespace GymDAL.Repo.Implementation.Workout
{
    public class WorkoutPlanItemRepository : Repository<WorkoutPlanItem>, IWorkoutPlanItemRepository
    {
        public WorkoutPlanItemRepository(GymDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

    }
}