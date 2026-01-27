using GymDAL.DB;
using GymDAL.Entities.Users;
using GymDAL.Repo.Abstract.Users;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GymDAL.Repo.Implementation.Users
{
    public class TempRegistrationRepository : Repository<TempRegistration>, ITempRegistrationRepository
    {
        public TempRegistrationRepository(GymDbContext context) : base(context)
        {
        }

        public async Task<TempRegistration> GetByEmailAsync(string email)
        {
            return await _context.TempRegistrations
                                 .FirstOrDefaultAsync(t => t.Email == email && t.RegistrationStatus == "Pending");
        }

        public async Task<TempRegistration> GetByOtpAsync(string email, string otp)
        {
            return await _context.TempRegistrations
                                 .FirstOrDefaultAsync(t => t.Email == email && t.OtpCode == otp && t.RegistrationStatus == "Pending");
        }

        public async Task<int> DeleteExpiredAsync()
        {
            var expiredRecords = await _context.TempRegistrations
                .Where(t => t.RegistrationStatus == "Pending" && t.OtpExpiry < DateTime.UtcNow)
                .ToListAsync();

            if (expiredRecords.Any())
            {
                _context.TempRegistrations.RemoveRange(expiredRecords);
                await _context.SaveChangesAsync();
            }

            return expiredRecords.Count;
        }
    }
}
