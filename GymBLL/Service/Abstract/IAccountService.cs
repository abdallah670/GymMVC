





using GymBLL.ModelVM.AccountVM;
using GymDAL.Entities.Users;

namespace MenoBLL.Service.Abstract
{
    public interface IAccountService
    {
         Task<Response<PasswordVM>> ChangePasswordAsync(PasswordVM model);
         Task<(Response<ApplicationUser>,string Role)>GetRole(string Email);
         Task<SignInResult> Login(LoginUserVM User);
         Task<bool> SignOut();
         public Task<ForgotPasswordResult> ForgotPasswordAsync(ForgotPasswordVM model);
            public Task<ResetPasswordResult> ResetPasswordAsync(ResetPasswordVM model);
        //Task UpdateSettings()





    }
}
