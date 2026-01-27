using GymBLL.ModelVM.Identity;
using GymBLL.ModelVM;
using GymBLL.Response;
using GymDAL.Entities.Users;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace GymBLL.Service.Abstract.Identity
{
    public interface IAccountService
    {
        Task<Response<PasswordVM>> ChangePasswordAsync(PasswordVM model);
        Task<(Response<ApplicationUser>, string Role)> GetRole(string Email);
        Task<SignInResult> Login(LoginUserVM User);
        Task<bool> SignOut();
        Task<ForgotPasswordResult> ForgotPasswordAsync(ForgotPasswordVM model);
        Task<ResetPasswordResult> ResetPasswordAsync(ResetPasswordVM model);
    }
}
