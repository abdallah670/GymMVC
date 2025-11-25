using AutoMapper;
using GymDAL.Repo.Abstract.Nutrition;
using System.Linq.Expressions;

namespace GymDAL.Repo.Implementation
{
    public class DietPlanAssignmentRepository : Repository<DietPlanAssignment>, IDietPlanAssignmentRepository
    {
        public DietPlanAssignmentRepository(GymDbContext context, IMapper mapper) : base(context, mapper)
        {
        }
        public override async Task<DietPlanAssignment> GetByIdAsync(int id)
        {
            try
            {
                return await _context.DietPlanAssignments
                                  // Include Member
                    .Include(d => d.DietPlan)             // Include DietPlan
                    .FirstOrDefaultAsync(d => d.Id == id);
            }
            catch (Exception ex)
            {
                // Log the exception here
                // _logger.LogError(ex, $"Error retrieving diet plan assignment with ID {id}");
                throw; // Re-throw to preserve the stack trace
            }
        }


    }
}