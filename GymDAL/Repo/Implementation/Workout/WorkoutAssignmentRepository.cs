using AutoMapper;
using GymDAL.Repo.Abstract.Workout;

namespace GymDAL.Repo.Implementation.Workout
{
    public class WorkoutAssignmentRepository : Repository<WorkoutAssignment>, IWorkoutAssignmentRepository
    {
        public WorkoutAssignmentRepository(GymDbContext context, IMapper mapper) : base(context, mapper)
        {
        }
        public override async Task<WorkoutAssignment> GetByIdAsync(int id)
        {
            try
            {
                var workoutAssignment = await _context.WorkoutAssignments
                        // Include Member navigation property
                    .Include(w => w.WorkoutPlan)   // Include WorkoutPlan if you have this relationship
                    .FirstOrDefaultAsync(w => w.Id == id);

                return workoutAssignment;
            }
            catch
            {
                throw;
            }
        }

        public async Task<WorkoutAssignment> GetDetailed(int? WorkoutasssignmentId)
        {
            try
            {
                return await _context.WorkoutAssignments
                    .Where(a=> a.IsActive&&a.Id== WorkoutasssignmentId)
                    .Include(a => a.WorkoutPlan)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching detailed workout assignment", ex);
            }
        }
    }
}
