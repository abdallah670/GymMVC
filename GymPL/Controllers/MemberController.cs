using Azure;
using GymBLL.ModelVM;
using GymBLL.ModelVM.Financial;
using GymBLL.ModelVM.Communication;
using GymBLL.ModelVM.Nutrition;
using GymBLL.ModelVM.Member;
using GymBLL.ModelVM.Trainer;
using GymBLL.ModelVM.Workout;
using GymBLL.Service.Abstract.Member;
using GymBLL.Service.Abstract.Nutrition;
using GymBLL.Service.Abstract.Workout;
using GymBLL.Service.Abstract.Financial;
using GymBLL.Service.Abstract.Communication;

using GymDAL.Entities.Users;
using GymDAL.Repo.Abstract;
using GymPL.Services;
using GymPL.Extensions;
using GymBLL.ModelVM.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;

namespace GymPL.Controllers
{
    [Authorize]
    
    public class MemberController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly ISubscriptionService _SubscriptionService;
        private readonly IMembershipService _membershipService;
        private readonly IPaymentService _paymentService;
        public readonly IFitnessGoalsService fitnessGoalsService;
        private readonly INotificationService _notificationService;
        private readonly IWorkoutAssignmentService _workoutAssignmentService;
        private readonly IDietPlanAssignmentService _dietPlanAssignmentService;
        private readonly IWorkoutPlanService _workoutPlanService;
        private readonly IDietPlanService _dietPlanService;
        private readonly IUnitOfWork _unitOfWork;
        private IFileUploadService _fileUploadService;
        private readonly IWorkoutPlanItemService _workoutPlanItemService;
        private readonly IDietPlanItemService _dietPlanItemService;
        private readonly IWeightLogService _weightLogService;
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWorkoutLogService _workoutLogService;
        private readonly IStripePaymentService _stripePaymentService;
        private const int DEFAULT_PAGE_SIZE = 6;
        public MemberController(IMemberService memberService,
            ISubscriptionService SubscriptionService, 
            IMembershipService membershipService ,
            IPaymentService paymentService,
            IFitnessGoalsService fitnessGoalsService,
            INotificationService notificationService,
            IWorkoutAssignmentService workoutAssignmentService,
            IDietPlanAssignmentService dietPlanAssignmentService,
            IWorkoutPlanItemService workoutPlanItemService,
            IDietPlanItemService dietPlanItemService,
            IWeightLogService weightLogService,
            IUnitOfWork unitOfWork, IWorkoutPlanService _workoutPlanService, IDietPlanService _dietPlanService,
            Microsoft.AspNetCore.Hosting.IWebHostEnvironment webHostEnvironment,  IFileUploadService _fileUploadService,
            UserManager<ApplicationUser> userManager,
            IWorkoutLogService workoutLogService,
            IStripePaymentService stripePaymentService
            )
        {
            _memberService = memberService;
            _SubscriptionService = SubscriptionService;
            _membershipService = membershipService;
            this._paymentService = paymentService;
            this.fitnessGoalsService = fitnessGoalsService;
            this._notificationService = notificationService;
            this._workoutAssignmentService = workoutAssignmentService;
            this._dietPlanAssignmentService = dietPlanAssignmentService;
            _workoutPlanItemService = workoutPlanItemService;
            _dietPlanItemService = dietPlanItemService;
            _weightLogService = weightLogService;
            _unitOfWork = unitOfWork;
            this._workoutPlanService=_workoutPlanService;
            this._dietPlanService=_dietPlanService;
            _webHostEnvironment = webHostEnvironment;
            this._fileUploadService=_fileUploadService;
            _userManager = userManager;
            _workoutLogService = workoutLogService;
            _stripePaymentService = stripePaymentService;
        }
        #region Member CRUD
        public async Task<IActionResult> Index(string view = "grid", int page = 1, int pageSize = DEFAULT_PAGE_SIZE)
        {

            try
            {
                if (view == "grid")
                    pageSize = 4;
                else
                    pageSize = DEFAULT_PAGE_SIZE;

                var response = await _memberService.GetPagedMembersAsync(page, pageSize);


                // Pass view type to layout
                ViewBag.ViewType = view;
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = response.Result.TotalPages;
                ViewBag.TotalItems = response.Result.TotalCount;


                return View(response.Result);
            }
            catch (Exception ex)
            {

                TempData["Error"] = "An error occurred while loading members.";
                return View(new PagedResult<MemberVM>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMemberPlanStatus(string memberId)
        {
            try
            {
                // Check workout plans using your existing service
                var response = await _SubscriptionService.CheckActivePlanAsync(memberId);

                if (response.ISHaveErrorOrnNot)
                {
                    // Return consistent error structure
                    return Json(new
                    {
                        success = false,
                        error = "Unable to check workout plans",
                        hasWorkoutPlan = false,
                        hasDietPlan = false
                    });
                }

                var hasWorkoutPlan = response.Result.hasWorkout;
                var hasDietPlan = response.Result.hasDiet;

                // Return consistent success structure
                return Json(new
                {
                    success = true,
                    hasWorkoutPlan = hasWorkoutPlan,
                    workoutAssignmentId = response.Result.workoutAssignmentId,
                    hasDietPlan = hasDietPlan,
                    dietPlanAssignmentId = response.Result.dietPlanAssignmentId,
                    error = ""
                });
            }
            catch (Exception ex)
            {
                
                return Json(new
                {
                    success = false,
                    error = "An error occurred while checking plan status",
                    hasWorkoutPlan = false,
                    hasDietPlan = false
                });
            }
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

        public async Task<IActionResult> Dashboard()
        {
            var memberId = User.GetUserId();
            
            ViewBag.HeaderType = "Member";
            
            var model = new MemberDashboardVM
            {
                MemberName = User.GetUserFullName(),
                HasWorkoutPlan = false,
                HasDietPlan = false
            };

            // Get Subscription Status
            var subscriptionResponse = await _SubscriptionService.GetByMemeberIdAsync(memberId);
            var sub = subscriptionResponse?.Result;

            if (sub != null)
            {
                var membershipResponse = await _membershipService.GetByIdAsync(sub.MembershipId);
                
                var daysRemaining = (sub.EndDate - DateTime.UtcNow).Days;
                var isExpired = daysRemaining < 0;

                model.SubscriptionStatus = new MemberDashboardSubscriptionVM
                {
                    SubscriptionId = sub.Id.ToString(),
                    MemberId = sub.MemberId,
                    MembershipType = membershipResponse.Result?.MembershipType ?? "Plan",
                    Status = sub.Status,
                    EndDate = sub.EndDate,
                    DaysRemaining = Math.Max(0, daysRemaining),
                    IsExpiringSoon = daysRemaining <= 7,
                    IsExpired = isExpired,
                    StatusBadgeClass = isExpired ? "badge-danger" : (daysRemaining <= 7 ? "badge-warning" : "badge-success")
                };

                // Check Workout Plan
                if (sub.WorkoutAssignmentVM != null && sub.WorkoutAssignmentVM.IsActive)
                {
                    model.HasWorkoutPlan = true;
                    model.TodayWorkoutName = sub.WorkoutAssignmentVM.WorkoutPlan?.Name ?? "Assigned Plan";
                }
                
                if (sub.DietPlanAssignmentVM != null && sub.DietPlanAssignmentVM.IsActive)
                {
                    model.HasDietPlan = true;
                    model.TodayDietName = sub.DietPlanAssignmentVM.DietPlan?.Name ?? "Assigned Plan";
                }
            }
            else
            {
                // If no active/unexpired sub found, try to get the latest one (even if expired) for the dashboard card
                var latestSubs = await _SubscriptionService.GetByMemberIdAsync(memberId);
                var latestSub = latestSubs.Result?.OrderByDescending(s => s.EndDate).FirstOrDefault();
                
                if (latestSub != null)
                {
                    var daysRemaining = (latestSub.EndDate - DateTime.UtcNow).Days;
                    bool isExpired = daysRemaining < 0 || latestSub.Status == "Expired" || latestSub.Status == "Cancelled";

                    model.SubscriptionStatus = new MemberDashboardSubscriptionVM
                    {
                        SubscriptionId = latestSub.Id.ToString(),
                        MemberId = latestSub.MemberId,
                        MembershipType = latestSub.MembershipType ?? "Plan",
                        Status = latestSub.Status,
                        EndDate = latestSub.EndDate,
                        DaysRemaining = Math.Max(0, daysRemaining),
                        IsExpiringSoon = false,
                        IsExpired = isExpired,
                        StatusBadgeClass = isExpired ? "badge-danger" : "badge-success"
                    };
                }
            }

            // Get Weight History - Ensure sorted by date for chart
            var weightHistoryResponse = await _weightLogService.GetHistoryAsync(memberId);
            if (!weightHistoryResponse.ISHaveErrorOrnNot)
            {
                model.WeightHistory = weightHistoryResponse.Result.OrderBy(w => w.DateRecorded).ToList();
            }
            else
            {
                model.WeightHistory = new List<WeightLogVM>();
            }

            // Get Total Workouts and Consistency
            var workoutHistoryResponse = await _workoutLogService.GetMemberHistoryAsync(memberId);
            model.TotalWorkouts = workoutHistoryResponse.Result?.Count() ?? 0;

            if (!workoutHistoryResponse.ISHaveErrorOrnNot && workoutHistoryResponse.Result != null)
            {
                var logs = workoutHistoryResponse.Result;
                var today = DateTime.UtcNow.Date;
                
                // Fix for Sunday (0) to treat Monday (1) as start of week
                int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
                var currentWeekStart = today.AddDays(-1 * diff).Date;

                for (int i = 3; i >= 0; i--)
                {
                    var weekStart = currentWeekStart.AddDays(-7 * i);
                    var weekEnd = weekStart.AddDays(6);
                    
                    // Count unique days with workouts or total workouts? Usually total workouts.
                    var count = logs.Count(l => l.Date.Date >= weekStart && l.Date.Date <= weekEnd);
                    
                    model.ConsistencyLabels.Add(i == 0 ? "This Week" : (i == 1 ? "Last Week" : $"{i} Weeks Ago"));
                    model.ConsistencyData.Add(count);
                }
            }

            return View(model);
        }
       
        public async Task<IActionResult> Details(string id, string view = "grid", int page = 1, int pageSize = 6, string returnUrl = null)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var memberResponse = await _memberService.GetMemberByIdAsync(id);

            if (memberResponse.ISHaveErrorOrnNot)
            {
                TempData["Error"] = "Member not found";
                return RedirectToReturnUrl(returnUrl, page, view, pageSize);
            }
            
            var subscriptionsResponse = await _SubscriptionService.GetByMemeberIdAsync(id);
            var Member = new MemberDietWorkoutPlansVM
            {
                Id = memberResponse.Result.Id,
                FullName = memberResponse.Result.FullName,
                Email = memberResponse.Result.Email,
                JoinDate = memberResponse.Result.JoinDate,
                CurrentWeight = memberResponse.Result.CurrentWeight,
                Height = memberResponse.Result.Height,
                FitnessGoal = subscriptionsResponse.Result?.MemberVM?.FitnessGoal,
                
                // Initialize empty collections if needed
                DietPlanAssignmentVM = subscriptionsResponse.Result?.DietPlanAssignmentVM,
                WorkoutAssignmentVM = subscriptionsResponse.Result?.WorkoutAssignmentVM,
                Age = memberResponse.Result.Age,
                ActivityLevel = memberResponse.Result.ActivityLevel,
                Gender = memberResponse.Result.Gender,
                Phone = memberResponse.Result.Phone,
            };
            if (Member.DietPlanAssignmentVM != null)
                Member.DietPlanAssignmentVM.DietPlan = subscriptionsResponse.Result?.DietPlanAssignmentVM?.DietPlan;
            if (Member.WorkoutAssignmentVM != null)
                Member.WorkoutAssignmentVM.WorkoutPlan = subscriptionsResponse.Result?.WorkoutAssignmentVM?.WorkoutPlan;

            ViewBag.HasDiet = subscriptionsResponse.Result?.DietPlanAssignmentVM != null;
            ViewBag.HasWorkout = subscriptionsResponse.Result?.WorkoutAssignmentVM != null;

            // Store return URL for navigation
            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = Url.Action("Index", new { page, view, pageSize });
            }
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.ReturnPage = page;
            ViewBag.ReturnView = view;
            ViewBag.ReturnPageSize = pageSize;

            return View(Member);
        }

   

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var memberId = User.GetUserId();
            var response = await _memberService.GetMemberByIdAsync(memberId);
            
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = "Member not found.";
                return RedirectToAction("Dashboard");
            }

            return View(response.Result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(MemberVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string profilePicturePath = model.ProfilePicture;

            // Handle file upload if new picture is provided
            if (model.ProfileImageFile != null && model.ProfileImageFile.Length > 0)
            {
                // Get current profile picture to delete old one
                var currentProfilePicture = User.GetProfilePicture();
                if (!string.IsNullOrEmpty(currentProfilePicture))
                {
                    _fileUploadService.DeleteProfilePicture(currentProfilePicture);
                }

                // Upload new picture
                profilePicturePath = await _fileUploadService.UploadProfilePictureAsync(
                    model.ProfileImageFile, model.Id);
            }
            model.ProfilePicture = profilePicturePath;

            var response = await _memberService.UpdateMemberAsync(model);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return View(model);
            }
            await UpdateUserClaims(model);
            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction("Profile", model);
        }
        private async Task UpdateUserClaims(MemberVM memberVM)
        {


            // Sign out and sign back in with updated claims
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

            await AddMemberClaimsAsync(memberVM);

        }
        private IActionResult RedirectToReturnUrl(string returnUrl, int page = 1, string view = "grid", int pageSize = 6)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            
            return RedirectToAction(nameof(Index), new { page, view, pageSize });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id, int page = 1, string view = "grid", int pageSize = 6, string returnUrl = null)
        {
            var response = await _memberService.DeleteMemberAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
            }
            else
            {
                TempData["Success"] = "Member deleted successfully!";
                
                // Check if the current page is now empty
                if (page > 1)
                {
                    var itemsOnPage = await _memberService.GetPagedMembersAsync(page, pageSize);
                    if (itemsOnPage.Result.Items.Count == 0)
                    {
                        page--; // Redirect to previous page
                    }
                }
            }
            return RedirectToAction(nameof(Index), new { page, view, pageSize });
        }
        #endregion
        #region Member Plans
        [HttpGet]
        public async Task<IActionResult> GetAssignedWorkoutPlan()
        {
            var memberId = User.GetUserId();
            var subscriptionResponse = await _SubscriptionService.GetByMemeberIdAsync(memberId);

            if (subscriptionResponse.ISHaveErrorOrnNot || 
                subscriptionResponse.Result == null || 
                subscriptionResponse.Result.WorkoutAssignmentVM == null)
            {
                // Check for expired subscription
                var allSubs = await _SubscriptionService.GetByMemberIdAsync(memberId);
                var latestSub = allSubs.Result?.OrderByDescending(s => s.EndDate).FirstOrDefault();
                
                if (latestSub != null && latestSub.EndDate < DateTime.UtcNow)
                {
                    TempData["Error"] = "Your subscription has expired. Please renew your subscription to access your workout plan.";
                    return RedirectToAction("RenewSubscription");
                }

                TempData["Error"] = "No active workout plan found.";
                return RedirectToAction("Dashboard");
            }

            var assignment = subscriptionResponse.Result.WorkoutAssignmentVM;
            // Ensure workout plan is loaded for the view
            if (assignment.WorkoutPlan == null && assignment.WorkoutPlanId > 0)
            {
                 var planResponse = await _workoutPlanService.GetWorkoutPlanByIdAsync(assignment.WorkoutPlanId);
                 if (!planResponse.ISHaveErrorOrnNot) assignment.WorkoutPlan = planResponse.Result;
            }

            return View(assignment);
           
        }

        [HttpGet]
        public async Task<IActionResult> GetAssignedDietPlan()
        {
            var memberId = User.GetUserId();
            var subscriptionResponse = await _SubscriptionService.GetByMemeberIdAsync(memberId);

            if (subscriptionResponse.ISHaveErrorOrnNot || 
                subscriptionResponse.Result == null || 
                subscriptionResponse.Result.DietPlanAssignmentVM == null)
            {
                // Check for expired subscription
                var allSubs = await _SubscriptionService.GetByMemberIdAsync(memberId);
                var latestSub = allSubs.Result?.OrderByDescending(s => s.EndDate).FirstOrDefault();
                
                if (latestSub != null && latestSub.EndDate < DateTime.UtcNow)
                {
                    TempData["Error"] = "Your subscription has expired. Please renew your subscription to access your diet plan.";
                    return RedirectToAction("RenewSubscription");
                }

                TempData["Error"] = "No active diet plan found.";
                return RedirectToAction("Dashboard");
            }

            var assignment = subscriptionResponse.Result.DietPlanAssignmentVM;
            // Ensure diet plan is loaded for the view
            if (assignment.DietPlan == null && assignment.DietPlanId > 0)
            {
                 var planResponse = await _dietPlanService.GetDietPlanByIdAsync(assignment.DietPlanId);
                 if (!planResponse.ISHaveErrorOrnNot) assignment.DietPlan = planResponse.Result;
            }
            return View(assignment);
        }
        #endregion
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactTrainer(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                TempData["Error"] = "Message cannot be empty.";
                return RedirectToAction("Dashboard");
            }

            var memberId = User.GetUserId();
            var memberName = User.Identity.Name;

            // Find all users with role 'Trainer'
            var trainers = await _userManager.GetUsersInRoleAsync("Trainer");
            
            if (trainers == null || !trainers.Any())
            {
                // Fallback to Admin or Manager if no trainers found
                var admins = await _userManager.GetUsersInRoleAsync("Admin");
                trainers = admins.Any() ? admins : await _userManager.GetUsersInRoleAsync("Manager");
            }

            if (trainers != null && trainers.Any())
            {
                foreach (var trainer in trainers)
                {
                   var notification = new NotificationVM
                   {
                       UserId = trainer.Id,
                       Type = "Info",
                       Message = $"Message from {memberName}: {message}",
                       Status = "Unread",
                       DeliveryMethod = "InApp",
                       SendTime = DateTime.UtcNow
                   };
                   await _notificationService.CreateAsync(notification);
                }
                TempData["Success"] = "Message sent to trainers successfully.";
            }
            else
            {
                 // If absolutely no one found, default to a system log or specific handling. 
                 // For now, warning is appropriate.
                 TempData["Warning"] = "Our trainers are currently unavailable. Please try again later.";
            }
            
            return RedirectToAction("Dashboard");
        }

        #region Subscription
        // Subscription Renewal
        [HttpGet]
        public async Task<IActionResult> RenewSubscription()
        {
            var memberId = User.GetUserId();
            
            // Get active subscription
            var subscriptionResponse = await _SubscriptionService.GetActiveSubscriptionByMemberIdAsync(memberId);
            if (subscriptionResponse.ISHaveErrorOrnNot)
            {
                TempData["Error"] = "No active subscription found.";
                return RedirectToAction("Dashboard");
            }

            var subscription = subscriptionResponse.Result;
            
            // Get membership details
            var membershipResponse = await _membershipService.GetByIdAsync(subscription.MembershipId);
            if (membershipResponse.ISHaveErrorOrnNot)
            {
                TempData["Error"] = "Membership plan not found.";
                return RedirectToAction("Dashboard");
            }

            var membership = membershipResponse.Result;

            var model = new RenewSubscriptionVM
            {
                SubscriptionId = subscription.Id,
                MemberId = memberId,
                MembershipId = subscription.MembershipId,
                MembershipType = membership.MembershipType,
                Amount = membership.Price,
                CurrentEndDate = subscription.EndDate,
                NewEndDate = subscription.EndDate.AddMonths(membership.DurationInMonths),
                DurationInMonths = membership.DurationInMonths
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RenewSubscription(RenewSubscriptionVM model)
        {
            try
            {
                var successUrl = Url.Action("Dashboard", "Member", null, Request.Scheme);
                var cancelUrl = Url.Action("RenewSubscription", "Member", new { id = model.SubscriptionId }, Request.Scheme);

                var checkoutUrl = await _stripePaymentService.CreateSubscriptionCheckoutSessionAsync(
                    User.FindFirst(ClaimTypes.Email)?.Value,
                    model.MembershipId,
                    model.MemberId,
                    "Renew",
                    successUrl,
                    cancelUrl
                );

                return Redirect(checkoutUrl);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred while initiating payment: {ex.Message}";
                return View(model);
            }
        }

        // Subscription Upgrade
        [HttpGet]
        public async Task<IActionResult> UpgradeSubscription()
        {
            var memberId = User.GetUserId();
            
            // Get active subscription
            var subscriptionResponse = await _SubscriptionService.GetActiveSubscriptionByMemberIdAsync(memberId);
            if (subscriptionResponse.ISHaveErrorOrnNot)
            {
                TempData["Error"] = "No active subscription found.";
                return RedirectToAction("Dashboard");
            }

            var subscription = subscriptionResponse.Result;
            
            // Get current membership
            var currentMembershipResponse = await _membershipService.GetByIdAsync(subscription.MembershipId);
            if (currentMembershipResponse.ISHaveErrorOrnNot)
            {
                TempData["Error"] = "Current membership plan not found.";
                return RedirectToAction("Dashboard");
            }

            var currentMembership = currentMembershipResponse.Result;

            // Get all active memberships for upgrade options
            var allMembershipsResponse = await _membershipService.GetActiveAsync();
            var availableUpgrades = allMembershipsResponse.Result
                .Where(m => m.Price > currentMembership.Price && m.Id != currentMembership.Id)
                .ToList();

            var model = new UpgradeSubscriptionVM
            {
                CurrentSubscriptionId = subscription.Id,
                MemberId = memberId,
                CurrentMembershipId = currentMembership.Id,
                CurrentMembershipType = currentMembership.MembershipType,
                CurrentPrice = currentMembership.Price,
                CurrentEndDate = subscription.EndDate,
                AvailableUpgrades = availableUpgrades
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpgradeSubscription(UpgradeSubscriptionVM model)
        {
            try
            {
                var successUrl = Url.Action("Dashboard", "Member", null, Request.Scheme);
                var cancelUrl = Url.Action("UpgradeSubscription", "Member", null, Request.Scheme);

                var checkoutUrl = await _stripePaymentService.CreateSubscriptionCheckoutSessionAsync(
                    User.FindFirst(ClaimTypes.Email)?.Value ,
                    model.NewMembershipId,
                    model.MemberId,
                    "Upgrade",
                    successUrl,
                    cancelUrl
                );

                return Redirect(checkoutUrl);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred while initiating payment: {ex.Message}";
                
                // Reload available upgrades
                var allMembershipsResponse = await _membershipService.GetActiveAsync();
                model.AvailableUpgrades = allMembershipsResponse.Result
                    .Where(m => m.Price > model.CurrentPrice && m.Id != model.CurrentMembershipId)
                    .ToList();
                return View(model);
            }
        }

        // Get Subscription Status for Dashboard
        [HttpGet]
        public async Task<IActionResult> GetSubscriptionStatus()
        {
            var memberId = User.GetUserId();
            
            var subscriptionResponse = await _SubscriptionService.GetActiveSubscriptionByMemberIdAsync(memberId);
            if (subscriptionResponse.ISHaveErrorOrnNot)
            {
                return Json(new { hasSubscription = false });
            }

            var subscription = subscriptionResponse.Result;
            var membershipResponse = await _membershipService.GetByIdAsync(subscription.MembershipId);
            
            var daysRemaining = (subscription.EndDate - DateTime.UtcNow).Days;
            var isExpiringSoon = daysRemaining <= 7;
            var isExpired = daysRemaining < 0;

            var statusVM = new SubscriptionStatusVM
            {
                SubscriptionId = subscription.Id,
                MemberId = memberId,
                MembershipType = membershipResponse.Result?.MembershipType ?? "Unknown",
                Status = subscription.Status,
                StartDate = subscription.StartDate,
                EndDate = subscription.EndDate,
                DaysRemaining = Math.Max(0, daysRemaining),
                IsExpiringSoon = isExpiringSoon,
                IsExpired = isExpired,
                CanRenew = true,
                CanUpgrade = !isExpired,
                RenewalPrice = membershipResponse.Result?.Price ?? 0,
                StatusBadgeClass = isExpired ? "badge-danger" : isExpiringSoon ? "badge-warning" : "badge-success",
                ProgressPercentage = CalculateProgressPercentage(subscription.StartDate, subscription.EndDate)
            };

            return Json(statusVM);
        }

        private string CalculateProgressPercentage(DateTime startDate, DateTime endDate)
        {
            var totalDays = (endDate - startDate).TotalDays;
            var daysElapsed = (DateTime.UtcNow - startDate).TotalDays;
            var percentage = Math.Min(100, Math.Max(0, (daysElapsed / totalDays) * 100));
            return percentage.ToString("F0");
        }
        [HttpGet]
        public async Task<IActionResult> Payments()
        {
            var memberId = User.GetUserId();
            var response = await _paymentService.GetByMemberIdAsync(memberId);
            
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = "Unable to retrieve payment history.";
                return View(new List<PaymentVM>());
            }

            return View(response.Result.OrderByDescending(p => p.PaymentDate).ToList());
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var memberId = User.GetUserId();
            var memberResponse = await _memberService.GetMemberByIdAsync(memberId);
            
            if (memberResponse.ISHaveErrorOrnNot)
            {
                TempData["Error"] = "Member not found.";
                return RedirectToAction("Dashboard");
            }

            var member = memberResponse.Result;
            var subscriptionResponse = await _SubscriptionService.GetByMemeberIdAsync(memberId);
            
            var model = new MemberProfileDisplayVM
            {
                MemberId = member.Id,
                FullName = member.FullName,
                Email = member.Email,
                JoinDate = member.JoinDate,
                CurrentWeight = (double)member.CurrentWeight,
                Height =(double) member.Height,
                FitnessGoal = member.FitnessGoal.GoalName, // Assuming this exists on MemberVM or we fetch it
                Gender = member.Gender,
                Age = member.Age,
                ProfilePicture=member.ProfilePicture
                
            };

            if (!subscriptionResponse.ISHaveErrorOrnNot && subscriptionResponse.Result != null)
            {
                var sub = subscriptionResponse.Result;
                var membershipResponse = await _membershipService.GetByIdAsync(sub.MembershipId);
                
                model.SubscriptionStatus = new GymBLL.ModelVM.Member.MemberDashboardSubscriptionVM
                {
                    SubscriptionId = sub.Id.ToString(),
                    MembershipType = membershipResponse.Result?.MembershipType ?? "Unknown",
                    Status = sub.Status,
                    StartDate = sub.StartDate,
                    EndDate = sub.EndDate,
                    DaysRemaining = (sub.EndDate - DateTime.UtcNow).Days
                };
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogWeight(double weight, string notes)
        {
            var memberId = User.GetUserId();
            var model = new WeightLogVM
            {
                MemberId = memberId,
                Weight = weight,
                Notes = notes,
                DateRecorded = DateTime.UtcNow
            };

            var response = await _weightLogService.LogWeightAsync(model);
            if (response.Result)
            {
                TempData["Success"] = "Weight logged successfully!";
            }
            else
            {
                TempData["Error"] = response.ErrorMessage;
            }

            return RedirectToAction("Dashboard");
        }

        #endregion
    }
}
