


using GymDAL.Repo.Abstract.Users;

namespace GymDAL.Repo.Implementation
{
    public class TrainerRepository : Repository<Trainer>, ITrainerRepository
    {
        GymDbContext gymDbContext;
        public TrainerRepository(GymDbContext context, IMapper mapper) : base(context,mapper)
        {
         
          
        }
    }
}