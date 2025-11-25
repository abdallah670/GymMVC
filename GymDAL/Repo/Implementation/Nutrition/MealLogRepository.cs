using AutoMapper;
using GymDAL.Repo.Abstract.Nutrition;
using System.Linq.Expressions;

namespace GymDAL.Repo.Implementation
{
    public class MealLogRepository : Repository<MealLog>, IMealLogRepository
    {
        public MealLogRepository(GymDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

    }
}