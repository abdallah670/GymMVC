using GymDAL.Entities.Users;
using GymDAL.Repo.Abstract.Users;
using GymDAL.Repo.Implementation.Core;
using Microsoft.EntityFrameworkCore;

namespace GymDAL.Repo.Implementation.Users
{
    public class WeightLogRepository : Repository<WeightLog>, IWeightLogRepository
    {
        public WeightLogRepository(GymDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<WeightLog>> GetWeightHistoryAsync(string memberId)
        {
            return await _context.WeightLogs
                .Where(wl => wl.MemberId == memberId)
                .OrderByDescending(wl => wl.DateRecorded)
                .ToListAsync();
        }
    }
}
