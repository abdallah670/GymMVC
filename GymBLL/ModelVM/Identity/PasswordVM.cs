using System.ComponentModel.DataAnnotations;

namespace GymBLL.ModelVM.Identity
{
    public class PasswordVM
    {
        public string Email { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
    public class ForgotPasswordVM
    {
        public string Email { get; set; }
    }
    public class PasswordResetModel
    {
        public string ResetLink { get; set; }
        public string UserName { get; set; }
    }
    public class ResetPasswordVM
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Token { get; set; }
    }

    public class ResetPasswordResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public IEnumerable<string> Errors { get; set; } = Enumerable.Empty<string>();
    }
    public class ForgotPasswordResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
