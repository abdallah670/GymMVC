using GymDAL.Entities.Users;
using System.Threading.Tasks;

namespace GymDAL.Repo.Abstract.Users
{
    public interface ITempRegistrationRepository : IRepository<TempRegistration>
    {
        Task<TempRegistration> GetByEmailAsync(string email);
        Task<TempRegistration> GetByOtpAsync(string email, string otp);
        Task<int> DeleteExpiredAsync();
    }
}
