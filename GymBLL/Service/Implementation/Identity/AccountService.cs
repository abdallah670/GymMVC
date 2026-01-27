using GymBLL.Response;
using GymBLL.ModelVM;
using GymBLL.ModelVM.Identity;
using GymBLL.Service.Abstract.Identity;

using GymDAL.Entities.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymBLL.Service.Abstract.Communication;
namespace GymBLL.Service.Implementation.Identity
{
    public class AccountService : IAccountService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly ILogger<AccountService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AccountService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IEmailService emailService, ILogger<AccountService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailService = emailService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<Response<PasswordVM>> ChangePasswordAsync(PasswordVM model)
        {
            try
            {
                var user = _userManager.FindByEmailAsync(model.Email).Result;
                if (user == null)
                {
                    return new Response<PasswordVM>(null, "User not found", true);
                }
                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                return new Response<PasswordVM>(model, result.Succeeded ? "Password changed successfully" : "Error changing password", !result.Succeeded);
            }
            catch (Exception ex)
            {
                return new Response<PasswordVM>(null, ex.Message, true);
            }
        }
        public async Task<SignInResult> Login(LoginUserVM User)
        {
            try
            {
                // 1. Try signing in with Email as UserName (default assumption)
                var result = await _signInManager.PasswordSignInAsync(User.Email, User.Password, User.RememberMe, false);
                
                if (result.Succeeded) return result;

                // 2. Fallback: Find user by Email to get the actual UserName
                var userEntity = await _userManager.FindByEmailAsync(User.Email);
                if (userEntity != null)
                {
                   // Retry with the actual UserName from DB
                   return await _signInManager.PasswordSignInAsync(userEntity.UserName, User.Password, User.RememberMe, false);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<bool> SignOut()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        Task<(Response<ApplicationUser>, string Role)> IAccountService.GetRole(string Email)
        {
            try
            {
                var user = _userManager.FindByEmailAsync(Email).Result;
                if (user == null)
                {
                    return Task.FromResult((new Response<ApplicationUser>(null, "User not found", true), string.Empty));
                }
                var role = _userManager.GetRolesAsync(user).Result.FirstOrDefault();
                return Task.FromResult((new Response<ApplicationUser>((ApplicationUser)user, "Role retrieved successfully", false), role));
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<ForgotPasswordResult> ForgotPasswordAsync(ForgotPasswordVM model)
        {
            try
            {
                _logger.LogInformation("Processing forgot password for email: {Email}", model.Email);
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {

                    // Don't reveal that the user doesn't exist for security
                    _logger.LogWarning("User not found for email: {Email}", model.Email);
                    return new ForgotPasswordResult
                    {
                        Success = true,
                        // Return true for security
                        Message = "If your email is registered, you will receive a password reset link shortly."
                    };
                }

                // Generate reset token
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                // Encode token for URL safety
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                // Generate reset link
                var resetLink = $"{GetBaseUrl()}/Account/ResetPassword?email={Uri.EscapeDataString(user.Email)}&token={encodedToken}";

                // Send email
                var emailSent = await _emailService.SendPasswordResetEmailAsync(user.Email, user.FullName, resetLink );


                if (emailSent)
                {
                    _logger.LogInformation("Password reset email sent to {Email}", user.Email);
                    return new ForgotPasswordResult
                    {
                        Success = true,
                        Message = "Password reset email has been sent. Please check your inbox."
                    };
                }
                else
                {
                    _logger.LogError("Failed to send password reset email to {Email}", user.Email);
                    return new ForgotPasswordResult
                    {
                        Success = false,
                        Message = "Failed to send reset email. Please try again later."
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ForgotPasswordAsync for email: {Email}", model.Email);
                return new ForgotPasswordResult
                {
                    Success = false,
                    Message = "An error occurred. Please try again."
                };
            }
        }
        public async Task<ResetPasswordResult> ResetPasswordAsync(ResetPasswordVM model)
        {
            try
            {
                _logger.LogInformation("Processing password reset for email: {Email}", model.Email);
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return new ResetPasswordResult
                    {
                        Success = false,
                        Message = "Invalid reset request."
                    };
                }

                // Decode the token
                var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));

                // Reset password
                var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.Password);


                if (result.Succeeded)
                {

                    // Update security stamp to invalidate existing sessions
                    await _userManager.UpdateSecurityStampAsync(user);
                    _logger.LogInformation("Password reset successful for email: {Email}", model.Email);
                    return new ResetPasswordResult
                    {
                        Success = true,
                        Message = "Password has been reset successfully. You can now login with your new password."
                    };
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogWarning("Password reset failed for {Email}: {Errors}", model.Email, errors);
                    return new ResetPasswordResult
                    {
                        Success = false,
                        Message = "Failed to reset password. The link may have expired or been used already.",
                        Errors = result.Errors?.Select(e => e.Description) ?? Enumerable.Empty<string>()
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ResetPasswordAsync for email: {Email}", model.Email);
                return new ResetPasswordResult
                {
                    Success = false,
                    Message = "An error occurred. Please try again."
                };
            }
        }
        private string GetBaseUrl()
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            if (request == null) return "https://localhost:5001";
            return $"{request.Scheme}://{request.Host}";
        }
    }
}



