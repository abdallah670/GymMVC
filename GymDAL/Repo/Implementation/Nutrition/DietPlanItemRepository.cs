using AutoMapper;
using GymDAL.Repo.Abstract.Nutrition;
using System.Linq.Expressions;

namespace GymDAL.Repo.Implementation
{
    public class DietPlanItemRepository : Repository<DietPlanItem>, IDietPlanItemRepository
    {
        public DietPlanItemRepository(GymDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

    }
}