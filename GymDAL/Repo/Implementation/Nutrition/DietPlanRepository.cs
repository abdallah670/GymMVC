using AutoMapper;
using GymDAL.Repo.Abstract.Nutrition;
using System.Linq.Expressions;

namespace GymDAL.Repo.Implementation
{
    public class DietPlanRepository : Repository<DietPlan>, IDietPlanRepository
    {
        public DietPlanRepository(GymDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        
        
    }
}