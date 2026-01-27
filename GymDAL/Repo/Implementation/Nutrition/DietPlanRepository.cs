using AutoMapper;
using GymDAL.Repo.Abstract.Nutrition;
using System.Linq.Expressions;

namespace GymDAL.Repo.Implementation.Nutrition
{
    public class DietPlanRepository : Repository<DietPlan>, IDietPlanRepository
    {
        public DietPlanRepository(GymDbContext context, IMapper mapper) : base(context, mapper)
        {
        }
        
        public override async Task<DietPlan> GetByIdAsync(int id)
        {
            try
            {
                    return await _context.DietPlans
                   .Include(w => w.DietPlanItems
                       .Where(d => d.DayNumber >= 1 && d.DayNumber <= 7)
                       .OrderBy(d => d.DayNumber))
                   .FirstOrDefaultAsync(w => w.Id == id);
            }
            catch
            {
                throw;
            }

        }

    }
}