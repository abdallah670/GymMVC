using GymBLL.ModelVM.AccountVM;
using GymBLL.ModelVM.User.Member;
using GymBLL.ModelVM.User.Trainer;
using GymBLL.Service.Abstract;
using GymBLL.Service.Implementation;
using GymDAL.Entities.Users;
using GymWeb.Extensions;
using MenoBLL.ModelVM.AccountVM;
using MenoBLL.Service.Abstract;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GymPL.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService accountService;
        private readonly ITrainerService trainerService;
        private readonly IMemberService memberService;


        public AccountController(IAccountService _accountService, ITrainerService trainerService, IMemberService memberService)
        {
            accountService = _accountService;
            this.trainerService = trainerService;
            this.memberService = memberService;

        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginUserVM model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var result = await accountService.Login(model);

                if (result.Succeeded)
                {
                    // Get the logged in user
                    var user = await accountService.GetRole(model.Email);
                    if (!user.Item1.ISHaveErrorOrnNot)
                    {
                        
                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        else if (user.Role== "Trainer")
                        {
                            var trainer =  await trainerService.GetTrainerByEmailAsync(model.Email);
                            // DEBUG: Check trainer data
                            Console.WriteLine($"Trainer FullName: {trainer.Result.FullName}");
                            Console.WriteLine($"Trainer Email: {trainer.Result.Email}");
                            await AddTrainerClaimsAsync(trainer.Result);
                         
                            return RedirectToAction("Dashboard", "Trainer");
                        }
                        else if (user.Role== "Member")
                        {
                            var Member = await memberService.GetMemberByEmailAsync(model.Email);
                            await AddMemberClaimsAsync(Member.Result);
                            return RedirectToAction("Dashboard", "Member");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            return View(model);
        }
        public IActionResult Settings()
        {
            return View();
        }
        public IActionResult SaveSettings()
        {
            return View();
        }
        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View(new PasswordVM());
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(PasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Get current user's email
                var userEmail = User.GetUserEmail();

                if (string.IsNullOrEmpty(userEmail))
                {
                    TempData["Error"] = "Unable to identify user. Please log in again.";
                    return View(model);
                }

                // Create the model for service layer
                var changePasswordModel = new PasswordVM
                {
                    Email = userEmail, // Add Email from current user
                    CurrentPassword = model.CurrentPassword,
                    NewPassword = model.NewPassword
                };

                // Call your service
                var result = await accountService.ChangePasswordAsync(changePasswordModel);

                if (!result.ISHaveErrorOrnNot)
                {
                    TempData["Success"] = result.ErrorMessage;

                    // Sign out user after password change (security best practice)
                    await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    TempData["Error"] = result.ErrorMessage;

                        ModelState.AddModelError(string.Empty, result.ErrorMessage);
                   
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
                return View(model);
            }
        }
        // GET: /Account/ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordVM());
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await accountService.ForgotPasswordAsync(model);

            if (result.Success)
            {
                TempData["Success"] = result.Message;
                return RedirectToAction("ForgotPasswordConfirmation");
            }
            else
            {
                TempData["Error"] = result.Message;
                return View(model);
            }
        }

        // GET: /Account/ForgotPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // GET: /Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Invalid reset link.";
                return RedirectToAction("Login");
            }

            var model = new ResetPasswordVM
            {
                Email = email,
                Token = token
            };

            return View(model);
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await accountService.ResetPasswordAsync(model);

            if (result.Success)
            {
                TempData["Success"] = result.Message;
                return RedirectToAction("ResetPasswordConfirmation");
            }
            else
            {
                if (result.Errors.Any(e => e.Contains("expired") || e.Contains("invalid")))
                {
                    TempData["Error"] = "The reset link has expired. Please request a new one.";
                    return RedirectToAction("ForgotPassword");
                }

                ModelState.AddModelError(string.Empty, result.Message);
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return View(model);
            }
        }

        // GET: /Account/ResetPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(); // Sign out from custom authentication
            await accountService.SignOut();
            return RedirectToAction("Index", "Home");
        }
        

        private async Task AddTrainerClaimsAsync(TrainerVM result)
        {
            var claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.NameIdentifier, result.Id));                      // Unique ID
            claims.Add(new Claim(ClaimTypes.Name, result.FullName ?? string.Empty));         // Username
            claims.Add(new Claim(ClaimTypes.Email, result.Email ?? string.Empty));           // Standard email
            claims.Add(new Claim("Email", result.Email ?? string.Empty));                     // Backup email
            claims.Add(new Claim("Phone", result.Phone ?? string.Empty));                     // Phone number
            claims.Add(new Claim(ClaimTypes.Role, "Trainer"));                               // Standard role
            claims.Add(new Claim("Role", "Trainer"));                                         // Backup role
            claims.Add(new Claim("ProfilePicture", result.ProfilePicture ?? string.Empty));   // Profile image
            claims.Add(new Claim("DisplayName", result.FullName ?? string.Empty));           // Display name

            var claimsIdentity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, claimsPrincipal);
        }

        private async Task AddMemberClaimsAsync(MemberVM result)
        {
            var claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.NameIdentifier, result.Id));                     // Unique ID
            claims.Add(new Claim(ClaimTypes.Name, result.FullName ?? string.Empty));        // Username
            claims.Add(new Claim("FullName", result.FullName ?? string.Empty));             // Backup full name
            claims.Add(new Claim(ClaimTypes.Email, result.Email ?? string.Empty));          // Standard email
            claims.Add(new Claim("Email", result.Email ?? string.Empty));                    // Backup email
            claims.Add(new Claim("Phone", result.Phone ?? string.Empty));                    // Phone number
            claims.Add(new Claim(ClaimTypes.Role, "Member"));                               // Standard role
            claims.Add(new Claim("Role", "Member"));                                         // Backup role
            claims.Add(new Claim("ProfilePicture", result.ProfilePicture ?? string.Empty));  // Profile image

            var claimsIdentity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, claimsPrincipal);
        }
    }
}
