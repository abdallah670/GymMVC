using GymDAL.DB;
using GymDAL.Entities.Users;

using GymDAL.Repo.Abstract.Users;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GymDAL.Repo.Implementation.Users
{
    public class TrainerReviewRepository : Repository<TrainerReview>, ITrainerReviewRepository
    {
        public TrainerReviewRepository(GymDbContext context) : base(context)
        {
        }

        public async Task<TrainerReview> GetDetailed(string memberId, string trainerId)
        {
            return await _context.TrainerReviews
                .Where(r => r.TrainerId == trainerId && r.MemberId == memberId)
                .Include(r => r.Trainer)
                .Include(r => r.Member)
                .FirstOrDefaultAsync();
        }
    }
}
