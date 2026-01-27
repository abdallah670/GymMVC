using GymBLL.ModelVM.Identity;
using GymBLL.ModelVM.Member;
using GymBLL.ModelVM.Trainer;
using GymBLL.Service.Abstract.Identity;
using GymBLL.Service.Implementation;
using GymDAL.Entities.Users;
using GymPL.Extensions;
using GymBLL.ModelVM;
using GymBLL.ModelVM.Identity;
using GymBLL.Service.Abstract.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GymBLL.Service.Abstract.Trainer;
using GymBLL.Service.Abstract.Member;
using GymBLL.Service.Abstract;
using GymBLL.Service.Abstract.Financial;
using GymBLL.ModelVM.Financial;

namespace GymPL.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService accountService;
        private readonly ITrainerService trainerService;
        private readonly IMemberService memberService;


        private readonly ITempRegistrationService _tempRegistrationService;
        private readonly IStripePaymentService _stripePaymentService;
        private readonly IMembershipService _membershipService;


        public AccountController(IAccountService _accountService, ITrainerService trainerService, IMemberService memberService, ITempRegistrationService tempRegistrationService, IStripePaymentService stripePaymentService, IMembershipService membershipService)
        {
            accountService = _accountService;
            this.trainerService = trainerService;
            this.memberService = memberService;
            _tempRegistrationService = tempRegistrationService;
            _stripePaymentService = stripePaymentService;
            _membershipService = membershipService;
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
        

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterStep1()
        {
            return View(new TempRegistrationVM());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterStep1(TempRegistrationVM model)
        {
            if (string.IsNullOrEmpty(model.Email))
            {
                ModelState.AddModelError("Email", "Email is required");
                return View(model);
            }

            var result = await _tempRegistrationService.InitiateregistrationAsync(model.Email);
            if (result.ISHaveErrorOrnNot)
            {
                ModelState.AddModelError("", result.ErrorMessage);
                return View(model);
            }

            // Redirect to OTP verification page with Email in TempData or query
            TempData["RegistrationEmail"] = model.Email;
            return RedirectToAction("VerifyOtp");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult VerifyOtp()
        {
            var email = TempData["RegistrationEmail"] as string;
            // If email is lost, maybe redirect to Step 1 or let user enter it?
            if (string.IsNullOrEmpty(email))
            
               return RedirectToAction("RegisterStep1");


            return View(new TempRegistrationVM { Email = email });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyOtp(string email, string otp)
        {
            var result = await _tempRegistrationService.VerifyOtpAsync(email, otp);
            if (result.ISHaveErrorOrnNot)
            {
                ModelState.AddModelError("", result.ErrorMessage);
                return View(new TempRegistrationVM { Email = email });
            }

            // OTP Verified -> Go to Step 2
            return RedirectToAction("RegisterStep2", new { email = email });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterStep2(string email)
        {
            if (string.IsNullOrEmpty(email)) return RedirectToAction("RegisterStep1");
            
            var existing = await _tempRegistrationService.GetByEmailAsync(email);
            if (existing.Result == null) return RedirectToAction("RegisterStep1");

            // Check if OTP verified? Service handles logic mostly, but good to check status
           // if (!existing.Result.IsOtpVerified) return RedirectToAction("VerifyOtp", new { email = email });

            return View(existing.Result);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterStep2(TempRegistrationVM model)
        {
             // Validate model
             if (!ModelState.IsValid) return View(model);

             var result = await _tempRegistrationService.UpdateDetailsAsync(model);
             if (result.ISHaveErrorOrnNot)
             {
                 ModelState.AddModelError("", result.ErrorMessage);
                 return View(model);
             }

             // If success -> Go to Pricing/Membership selection (or directly complete if pricing is next)
             // For now, assume Step 3 is Membership Selection
             return RedirectToAction("MembershipSelection", new { email = model.Email });
        }
        
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> MembershipSelection(string email)
        {
              ViewBag.Email = email;
              var memberships = await _membershipService.GetActiveAsync();
              if (memberships.ISHaveErrorOrnNot)
              {
                   ModelState.AddModelError("", "Failed to load membership plans.");
                   return View(new List<MembershipVM>());
              }
              return View(memberships.Result);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MembershipSelection(string email, int membershipId)
        {
             // Get temp registration
             var tempReg = await _tempRegistrationService.GetByEmailAsync(email);
             if (tempReg.Result == null) return RedirectToAction("RegisterStep1");

             var successUrl = Url.Action("PaymentSuccess", "Account", null, Request.Scheme);
             var cancelUrl = Url.Action("MembershipSelection", "Account", new { email = email }, Request.Scheme);

             var checkoutUrl = await _stripePaymentService.CreateCheckoutSessionAsync(
                 email, 
                 membershipId, 
                 tempReg.Result.Id, 
                 successUrl, 
                 cancelUrl);

             return Redirect(checkoutUrl);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult PaymentSuccess(string session_id)
        {
            // Display success message. Actual account creation happens via webhook.
            ViewBag.SessionId = session_id;
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GoogleLogin(string returnUrl = null)
        {
            var redirectUrl = Url.Action("GoogleResponse", "Account", new { returnUrl });
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, "Google");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleResponse(string returnUrl = null)
        {
            var info = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
            if (info?.Principal == null)
            {
                return RedirectToAction("Login");
            }

            var email = System.Security.Claims.PrincipalExtensions.FindFirstValue(info.Principal, ClaimTypes.Email);
            var name = System.Security.Claims.PrincipalExtensions.FindFirstValue(info.Principal, ClaimTypes.Name);

            // Check if user already exists
            var existingMember = await memberService.GetMemberByEmailAsync(email);
            if (existingMember.Result != null)
            {
                // User exists, sign them in
                await AddMemberClaimsAsync(existingMember.Result);
                return RedirectToAction("Dashboard", "Member");
            }

            // New user - start registration flow with pre-filled email
            var result = await _tempRegistrationService.InitiateregistrationAsync(email);
            if (!result.ISHaveErrorOrnNot)
            {
                // Mark OTP as verified since Google already verified the email
                await _tempRegistrationService.VerifyOtpAsync(email, result.Result.OtpCode);
            }

            return RedirectToAction("RegisterStep2", new { email = email });
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
