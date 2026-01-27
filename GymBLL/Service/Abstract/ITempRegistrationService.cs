using GymBLL.ModelVM;
using GymBLL.Response;
using System.Threading.Tasks;

namespace GymBLL.Service.Abstract
{
    public interface ITempRegistrationService
    {
        Task<Response<TempRegistrationVM>> InitiateregistrationAsync(string email);
        Task<Response<TempRegistrationVM>> VerifyOtpAsync(string email, string otp);
        Task<Response<TempRegistrationVM>> UpdateDetailsAsync(TempRegistrationVM model);
        Task<Response<TempRegistrationVM>> GetByEmailAsync(string email);
        Task<Response<bool>> CompleteRegistrationAsync(int tempRegistrationId);
        Task<Response<TempRegistrationVM>> GetByIdAsync(int tempRegistrationId);
    }
}
