using Azure;
using GymBLL.ModelVM;
using GymBLL.ModelVM.External;
using GymBLL.ModelVM.Notification;
using GymBLL.ModelVM.Nutrition;
using GymBLL.ModelVM.User.Member;
using GymBLL.ModelVM.Workout;
using GymBLL.Service.Abstract;
using GymBLL.Service.Implementation;
using GymDAL.Entities.External;
using GymDAL.Repo.Abstract;
using GymPL.ViewModels;
using GymWeb.Extensions;
using MenoBLL.ModelVM.AccountVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;

namespace GymPL.Controllers
{
    
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
        private readonly IWorkoutPlanItemService _workoutPlanItemService;
        private readonly IDietPlanItemService _dietPlanItemService;
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
            IUnitOfWork unitOfWork, IWorkoutPlanService _workoutPlanService, IDietPlanService _dietPlanService
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
            _unitOfWork = unitOfWork;
            this._workoutPlanService=_workoutPlanService;
            this._dietPlanService=_dietPlanService;
        }

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
                    hasDietPlan = hasDietPlan,
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
        [HttpGet]
        public async Task<IActionResult> Complete()
        {
            var memberId = User.GetUserId();

            // Check if profile already completed
            if (await _memberService.HasCompletedProfileAsync(memberId))
            {
                return RedirectToAction("Dashboard", "Member");
            }
            var FitnessGoals = (await fitnessGoalsService.GetAllFitnessGoalsAsync()).Result;
            var model = new MemberProfileVM { Id =memberId};
            ViewBag.FitnessGoals = FitnessGoals;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(MemberProfileVM model)
        {
            var memberResponse = await _memberService.GetMemberByIdAsync(model.Id);
            if (!memberResponse.ISHaveErrorOrnNot)
            {
                // If member exists, set the FullName and Email from database
                // This prevents validation errors for hidden fields
                var member = memberResponse.Result;
                model.FullName = member.FullName;
                model.Email = member.Email;
                

            }

            if (!ModelState.IsValid)
            {
                var FitnessGoals = (await fitnessGoalsService.GetAllFitnessGoalsAsync()).Result;
                ViewBag.FitnessGoals = FitnessGoals;
                return View(model);
            }
            model.HasCompletedProfile = true;
           
            var response = await _memberService.CompleteProfileAsync(model);
            if (response.ISHaveErrorOrnNot)
            {
                var FitnessGoals = (await fitnessGoalsService.GetAllFitnessGoalsAsync()).Result;
                TempData["Error"] = response.ErrorMessage;
                ViewBag.FitnessGoals = FitnessGoals;
                return View(model);
            }

            TempData["Success"] = "Profile completed successfully! Welcome to MenoPro!";
            return RedirectToAction("Dashboard", "Member");
        }


        public async Task<IActionResult> Dashboard()
        {
            var memberId = User.GetUserId();
            if (await _memberService.HasCompletedProfileAsync(memberId) == false)
            {
                return RedirectToAction("Complete", "Member");
            }
            
            ViewBag.HeaderType = "Member";
            
            var model = new MemberDashboardVM
            {
                MemberName = User.GetUserFullName(),
                HasWorkoutPlan = false,
                HasDietPlan = false
            };

            // Get Subscription Status
            var subscriptionResponse = await _SubscriptionService.GetByMemeberIdAsync(memberId);
            if (!subscriptionResponse.ISHaveErrorOrnNot)
            {
                var sub = subscriptionResponse.Result;
                var membershipResponse = await _membershipService.GetByIdAsync(sub.MembershipId);
                
                var daysRemaining = (sub.EndDate - DateTime.UtcNow).Days;
                var isExpired = daysRemaining < 0;

                model.SubscriptionStatus = new MemberDashboardSubscriptionVM
                {
                    SubscriptionId = sub.Id.ToString(), // Converted to string to match VM
                    MemberId = sub.MemberId,
                    MembershipType = membershipResponse.Result?.MembershipType ?? "Unknown",
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

            return View(model);
        }
        public  async Task< IActionResult> Create()
        {
             var response = await _membershipService.GetAllAsync();
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return RedirectToAction(nameof(Index));
            }
           
            ViewBag.Memberships = response.Result;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MemberDetailsVM model)
        {
            // Get memberships for validation and display
            var membershipsResponse = await _membershipService.GetAllAsync();
            if (membershipsResponse.ISHaveErrorOrnNot)
            {
                TempData["Error"] = membershipsResponse.ErrorMessage;
                return View(model);
            }
            var memberships = membershipsResponse.Result;
            ViewBag.Memberships = memberships;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Start transaction
                 _unitOfWork.BeginTransaction();

                // 1. Register Member
                var response = await _memberService.Register(model);
                if (!response.Succeeded)
                {
                     _unitOfWork.RollbackTransaction();
                    TempData["Error"] = response.Errors;
                    return View(model);
                }

                // 2. Get the created member
                var member = await _memberService.GetMemberByEmailAsync(model.Email);
                if (member.ISHaveErrorOrnNot)
                {
                    _unitOfWork.RollbackTransaction();
                    TempData["Error"] = member.ErrorMessage;
                    return View(model);
                }

                // 3. Find selected membership
                var selectedMembership = memberships.FirstOrDefault(m => m.Id == model.MembershipId);
                if (selectedMembership == null)
                {
                    _unitOfWork.RollbackTransaction();
                    TempData["Error"] = "Selected membership plan not found";
                    return View(model);
                }

                // 4. Create Payment
                var paymentModel = new PaymentVM
                {
                    MemberId = member.Result.Id,
                    Amount = selectedMembership.Price,
                    PaymentDate = DateTime.Now,
                    PaymentMethod = "Credit Card",

                };

                var payment = await _paymentService.CreateAsync(paymentModel);
                if (payment.ISHaveErrorOrnNot)
                {
                    _unitOfWork.RollbackTransaction();
                    TempData["Error"] = payment.ErrorMessage;
                    return View(model);
                }

                // 5. Create Subscription
                var subscriptionModel = new SubscriptionVM
                {
                    MemberId = member.Result.Id,
                    MembershipId = model.MembershipId,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddMonths(1),
                    PaymentId = payment.Result.Id,
                    Status = "Active",
                    MemberName = model.FullName,
                    MembershipType = selectedMembership.MembershipType
                };

                var subscription = await _SubscriptionService.CreateAsync(subscriptionModel);
                if (subscription.ISHaveErrorOrnNot)
                {
                    _unitOfWork.RollbackTransaction();
                    TempData["Error"] = subscription.ErrorMessage;
                    return View(model);
                }

                // 6. Commit transaction if everything succeeds
                await _unitOfWork.CommitTransactionAsync();

                TempData["Success"] = "Member created successfully!";
                return RedirectToAction("Dashboard");
            }
            catch (Exception ex)
            {
                // Rollback transaction on any exception
                _unitOfWork.RollbackTransaction();
                TempData["Error"] = $"An error occurred: {ex.Message}";
                return View(model);
            }
        }
        public async Task<IActionResult> Details(string id, string view = "grid", int pageSize = 6, string returnUrl = null)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var memberResponse = await _memberService.GetMemberByIdAsync(id);

            if (memberResponse.ISHaveErrorOrnNot)
            {
                TempData["Error"] = "Member not found";
                return RedirectToAction("Index");
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
                FitnessGoal = subscriptionsResponse.Result.MemberVM?.FitnessGoal
                ,
                // Initialize empty collections if needed
                DietPlanAssignmentVM = subscriptionsResponse.Result.DietPlanAssignmentVM,
                WorkoutAssignmentVM = subscriptionsResponse.Result.WorkoutAssignmentVM,


            };
            if (Member.DietPlanAssignmentVM != null)
                Member.DietPlanAssignmentVM.DietPlan = subscriptionsResponse.Result.DietPlanAssignmentVM?.DietPlan;
            if (Member.WorkoutAssignmentVM != null)
                Member.WorkoutAssignmentVM.WorkoutPlan = subscriptionsResponse.Result.WorkoutAssignmentVM?.WorkoutPlan;

            ViewBag.hasDiet = subscriptionsResponse.Result.DietPlanAssignmentVM != null ? true : false;
            ViewBag.hasWorkout = subscriptionsResponse.Result.WorkoutAssignmentVM != null ? true : false;


            return View(Member);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id, int page = 1, string view = "grid", int pageSize = 6, string returnUrl = null)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var response = await _memberService.GetMemberByIdAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return RedirectToAction(nameof(Index), new { page, view, pageSize });
            }
            
            // If returnUrl is not explicitly provided, construct it from params
            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = Url.Action("Index", new { page, view, pageSize });
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(response.Result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MemberVM model, int page = 1, string view = "grid", int pageSize = 6, string returnUrl = null)
        {
            if (!ModelState.IsValid) 
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            var response = await _memberService.UpdateMemberAsync(model);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            TempData["Success"] = "Member updated successfully!";
            
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            
            return RedirectToAction(nameof(Index));
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
            }
            return RedirectToAction(nameof(Index));
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
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Create payment
                var paymentModel = new PaymentVM
                {
                    MemberId = model.MemberId,
                    Amount = model.Amount,
                    PaymentMethod = model.PaymentMethod,
                    PaymentType = "Subscription Renewal",
                    Description = $"Renewal of {model.MembershipType} subscription",
                    Status = "Completed",
                    ProcessedDate = DateTime.UtcNow,
                    BillingName = model.BillingName,
                    BillingAddress = model.BillingAddress,
                    BillingEmail = model.BillingEmail,
                    Notes = model.Notes
                };

                var paymentResponse = await _paymentService.CreateAsync(paymentModel);
                if (paymentResponse.ISHaveErrorOrnNot)
                {
                    TempData["Error"] = paymentResponse.ErrorMessage;
                    return View(model);
                }

                // Renew subscription
                var renewResponse = await _SubscriptionService.RenewSubscriptionAsync(model.SubscriptionId);
                if (renewResponse.ISHaveErrorOrnNot)
                {
                    TempData["Error"] = renewResponse.ErrorMessage;
                    return View(model);
                }

                // Send notification
                var notification = new NotificationVM
                {
                    UserId = model.MemberId,
                    Type = "SubscriptionRenewal",
                    Message = $"Your {model.MembershipType} subscription has been renewed successfully! New expiry date: {model.NewEndDate:MMM dd, yyyy}",
                    Status = "Unread",
                    DeliveryMethod = "InApp"
                };
                await _notificationService.CreateAsync(notification);

                TempData["Success"] = "Subscription renewed successfully!";
                return RedirectToAction("Dashboard");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
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
            if (!ModelState.IsValid)
            {
                // Reload available upgrades
                var allMembershipsResponse = await _membershipService.GetActiveAsync();
                model.AvailableUpgrades = allMembershipsResponse.Result
                    .Where(m => m.Price > model.CurrentPrice && m.Id != model.CurrentMembershipId)
                    .ToList();
                return View(model);
            }

            try
            {
                // Get new membership details
                var newMembershipResponse = await _membershipService.GetByIdAsync(model.NewMembershipId);
                if (newMembershipResponse.ISHaveErrorOrnNot)
                {
                    TempData["Error"] = "Selected membership plan not found.";
                    return View(model);
                }

                var newMembership = newMembershipResponse.Result;
                model.PriceDifference = newMembership.Price - model.CurrentPrice;

                // Create payment for upgrade
                var paymentModel = new PaymentVM
                {
                    MemberId = model.MemberId,
                    Amount = model.PriceDifference,
                    PaymentMethod = model.PaymentMethod,
                    PaymentType = "Subscription Upgrade",
                    Description = $"Upgrade from {model.CurrentMembershipType} to {newMembership.MembershipType}",
                    Status = "Completed",
                    ProcessedDate = DateTime.UtcNow,
                    BillingName = model.BillingName,
                    BillingAddress = model.BillingAddress,
                    BillingEmail = model.BillingEmail,
                    Notes = model.Notes
                };

                // Upgrade subscription
                var upgradeResponse = await _SubscriptionService.UpgradeSubscriptionAsync(
                    model.CurrentSubscriptionId, 
                    model.NewMembershipId, 
                    paymentModel);
                
                if (upgradeResponse.ISHaveErrorOrnNot)
                {
                    TempData["Error"] = upgradeResponse.ErrorMessage;
                    return View(model);
                }

                // Send notification
                var notification = new NotificationVM
                {
                    UserId = model.MemberId,
                    Type = "SubscriptionUpgrade",
                    Message = $"Congratulations! Your subscription has been upgraded to {newMembership.MembershipType}. Enjoy your enhanced benefits!",
                    Status = "Unread",
                    DeliveryMethod = "InApp"
                };
                await _notificationService.CreateAsync(notification);

                TempData["Success"] = "Subscription upgraded successfully!";
                return RedirectToAction("Dashboard");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
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

        // Get Assigned Workout Plans
        //[HttpGet]
        //public async Task<IActionResult> GetAssignedWorkoutPlans()
        //{
        //    var memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
        //    var workoutAssignmentsResponse = await _workoutAssignmentService.GetByMemberIdAsync(memberId);
            
        //    if (workoutAssignmentsResponse.ISHaveErrorOrnNot || workoutAssignmentsResponse.Result == null)
        //    {
        //        return Json(new { hasPlans = false });
        //    }

        //    var activeAssignments = workoutAssignmentsResponse.Result
        //        .Where(w => w.IsActive && w.EndDate >= DateTime.UtcNow)
        //        .OrderByDescending(w => w.StartDate)
        //        .ToList();

        //    return Json(new { hasPlans = activeAssignments.Any(), plans = activeAssignments });
        //}

        [HttpGet]
        public async Task<IActionResult> ViewTodayWorkout()
        {
            var memberId = User.GetUserId();
            var subscriptionResponse = await _SubscriptionService.GetByMemeberIdAsync(memberId);
            
            if (subscriptionResponse.ISHaveErrorOrnNot || 
                subscriptionResponse.Result?.WorkoutAssignmentVM == null || 
                !subscriptionResponse.Result.WorkoutAssignmentVM.IsActive)
            {
                TempData["Error"] = "No active workout plan found.";
                return RedirectToAction("Dashboard");
            }

            var assignment = subscriptionResponse.Result.WorkoutAssignmentVM;
            var workoutPlanId = assignment.WorkoutPlanId;
           
            // Calculate Day Number (1-7) based on StartDate
            // Assuming 7-day cycle. If strictly 1-7 mapped to Mon-Sun, use DateTime.Now.DayOfWeek
            // Let's use DayOfWeek for now as it's common for "Today's Workout"
            // DayOfWeek: Sunday=0, Monday=1... Saturday=6. 
            // Map to 1-7: Sunday=7, Monday=1, ... Saturday=6
            
            int dayNumber = (int)DateTime.Now.DayOfWeek;
            if (dayNumber == 0) dayNumber = 7; 

            var allItemsResponse = await _workoutPlanItemService.GetByWorkoutPlanIdAsync(workoutPlanId);
            
            if (allItemsResponse.ISHaveErrorOrnNot)
            {
                TempData["Error"] = "Unable to load workout details.";
                return RedirectToAction("Dashboard");
            }

            var todayItems = allItemsResponse.Result
                .Where(i => i.DayNumber == dayNumber && i.IsActive)
                .OrderBy(i => i.Id) // Or any ordering logic
                .ToList();

            ViewBag.PlanName = assignment.WorkoutPlan.Name;
            ViewBag.DayName = DateTime.Now.DayOfWeek.ToString();
            ViewBag.Date = DateTime.Now.ToString("D");

            return View(todayItems);
        }

        [HttpGet]
        public async Task<IActionResult> TodayMeal()
        {
            var memberId = User.GetUserId();
            var subscriptionResponse = await _SubscriptionService.GetByMemeberIdAsync(memberId);
            
            if (subscriptionResponse.ISHaveErrorOrnNot || 
                subscriptionResponse.Result?.DietPlanAssignmentVM == null || 
                !subscriptionResponse.Result.DietPlanAssignmentVM.IsActive)
            {
                TempData["Error"] = "No active diet plan found.";
                return RedirectToAction("Dashboard");
            }

            var assignment = subscriptionResponse.Result.DietPlanAssignmentVM;
            var dietPlanId = assignment.DietPlanId;
            
            // Calculate Day Number relative to start date
            var daysElapsed = (DateTime.Now.Date - assignment.StartDate.Date).Days;
            
            // If internal cycle is needed, modulo by plan duration? 
            // For now, assume linear: Day 1, Day 2...
            // If daysElapsed exceeds plan length, maybe loop or show "Complete"?
            // Let's assume loop for diet plans often. 
            // But DietPlanVM has DurationDays.
             var duration = assignment.DietPlan?.DurationDays ?? 30;
             if (duration == 0) duration = 30; // Fallback

             // 0-indexed daysElapsed mapped to 1-indexed DayNumber
             // Modulo duration to loop
             int dayNumber = (daysElapsed % duration) + 1;

            var allItemsResponse = await _dietPlanItemService.GetByDietPlanIdAsync(dietPlanId);
            
            if (allItemsResponse.ISHaveErrorOrnNot)
            {
                TempData["Error"] = "Unable to load diet details.";
                return RedirectToAction("Dashboard");
            }

            var todayItems = allItemsResponse.Result
                .Where(i => i.DayNumber == dayNumber && i.IsActive)
                .OrderBy(i => i.MealType) // Simple sort
                .ToList();

            ViewBag.PlanName = assignment.DietPlan.Name;
            ViewBag.DayNumber = dayNumber;
            ViewBag.Date = DateTime.Now.ToString("D");

            return View(todayItems);
        }
        #endregion
        #region Workout Plans Assignment

        [HttpGet]
        public async Task<IActionResult> AssignWorkoutPlan(string memberId)
        {
            if (string.IsNullOrEmpty(memberId)) return NotFound();

            var memberResponse = await _memberService.GetMemberByIdAsync(memberId);
            if (memberResponse.ISHaveErrorOrnNot)
            {
                TempData["Error"] = "Member not found";
                return RedirectToAction("Dashboard");
            }

            var workoutPlansResponse = await _workoutPlanService.GetAllWorkoutPlansAsync();

            ViewBag.Member = memberResponse.Result;
            ViewBag.WorkoutPlans = workoutPlansResponse.Result ?? new List<WorkoutPlanVM>();

            var model = new WorkoutAssignmentVM
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1),
                IsActive = true
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignWorkoutPlan(WorkoutAssignmentVM model, string memberId)
        {
            if (ModelState.IsValid)
            {
                var response = await _workoutAssignmentService.CreateAsync(model);
                if (!response.ISHaveErrorOrnNot)
                {
                    // Update Subscription with new Assignment
                    var resulteResponse = await _SubscriptionService.GetByMemeberIdAsync(memberId);
                    if (!resulteResponse.ISHaveErrorOrnNot)
                    {
                        // Create a VM with just the ID to update the relationship
                        resulteResponse.Result.WorkoutAssignmentVM = new WorkoutAssignmentVM { Id = response.Result.Id };
                        await _SubscriptionService.UpdateAsync(resulteResponse.Result);
                    }

                    TempData["Success"] = "Workout plan assigned successfully!";
                    return RedirectToAction("MemberDetails", new { id = memberId });
                }
                TempData["Error"] = response.ErrorMessage;
            }

            // Reload data for view
            var memberResponse = await _memberService.GetMemberByIdAsync(memberId);
            ViewBag.Member = memberResponse.Result;
            var workoutPlansResponse = await _workoutPlanService.GetAllWorkoutPlansAsync();
            ViewBag.WorkoutPlans = workoutPlansResponse.Result ?? new List<WorkoutPlanVM>();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditWorkoutPlan(int assignmentId, string memberId)
        {
            if (string.IsNullOrEmpty(memberId)) return NotFound();

            var assignmentResponse = await _workoutAssignmentService.GetByIdAsync(assignmentId);
            if (assignmentResponse.ISHaveErrorOrnNot)
            {
                TempData["Error"] = "Workout assignment not found";
                return RedirectToAction("MemberDetails", new { id = memberId });
            }

            var memberResponse = await _memberService.GetMemberByIdAsync(memberId);
            var workoutPlansResponse = await _workoutPlanService.GetAllWorkoutPlansAsync();

            ViewBag.Member = memberResponse.Result;
            ViewBag.MemberId = memberId;
            ViewBag.WorkoutPlans = workoutPlansResponse.Result ?? new List<WorkoutPlanVM>();

            return View(assignmentResponse.Result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditWorkoutPlan(WorkoutAssignmentVM model, string memberId)
        {
            if (ModelState.IsValid)
            {
                var response = await _workoutAssignmentService.UpdateAsync(model);

                if (!response.ISHaveErrorOrnNot)
                {
                    TempData["Success"] = "Workout plan updated successfully!";
                    return RedirectToAction("MemberDetails", new { id = memberId });
                }

                TempData["Error"] = response.ErrorMessage;
            }

            var memberResponse = await _memberService.GetMemberByIdAsync(memberId);
            var workoutPlansResponse = await _workoutPlanService.GetAllWorkoutPlansAsync();
            ViewBag.Member = memberResponse.Result;
            ViewBag.MemberId = memberId;
            ViewBag.WorkoutPlans = workoutPlansResponse.Result ?? new List<WorkoutPlanVM>();
            return View(model);
        }

        #endregion
        #region Member Diet Plan Editing


        // Create Diet Plan for Member
        [HttpGet]
        public async Task<IActionResult> AssignDietPlan(string memberId)
        {
            if (string.IsNullOrEmpty(memberId))
                return NotFound();

            var memberResponse = await _memberService.GetMemberByIdAsync(memberId);
            if (memberResponse.ISHaveErrorOrnNot)
            {

                TempData["Error"] = "Member not found";
                return RedirectToAction("MyMembers");
            }

            // Get available diet plans
            var dietPlansResponse = await _dietPlanService.GetAllDietPlansAsync();

            ViewBag.Member = memberResponse.Result;
            ViewBag.DietPlans = dietPlansResponse.Result ?? new List<DietPlanVM>();

            var model = new DietPlanAssignmentVM
            {

                MemberName = memberResponse.Result.FullName,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1),
                IsActive = true
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignDietPlan(DietPlanAssignmentVM model, string memberId)
        {
            if (!ModelState.IsValid)
            {
                var memberResponse = await _memberService.GetMemberByIdAsync(memberId);
                var dietPlansResponse = await _dietPlanService.GetAllDietPlansAsync();
                ViewBag.Member = memberResponse.Result;
                ViewBag.DietPlans = dietPlansResponse.Result ?? new List<DietPlanVM>();
                return View(model);
            }

            var response = await _dietPlanAssignmentService.CreateAsync(model);

            if (response.ISHaveErrorOrnNot)
            {
                var memberResponse = await _memberService.GetMemberByIdAsync(memberId);
                if (memberResponse.ISHaveErrorOrnNot)
                {

                    TempData["Error"] = "Member not found";
                    return RedirectToAction("MyMembers");
                }

                // Get available diet plans
                var dietPlansResponse = await _dietPlanService.GetAllDietPlansAsync();

                ViewBag.Member = memberResponse.Result;
                ViewBag.DietPlans = dietPlansResponse.Result ?? new List<DietPlanVM>();
                TempData["Error"] = response.ErrorMessage;
                return View(model);
            }

            var resulteResponse = await _SubscriptionService.GetByMemeberIdAsync(memberId);
            resulteResponse.Result.DietPlanAssignmentVM = new DietPlanAssignmentVM();
            resulteResponse.Result.DietPlanAssignmentVM.Id = response.Result.Id;

            var Updated = await _SubscriptionService.UpdateAsync(resulteResponse.Result);
            if (Updated.ISHaveErrorOrnNot)
            {
                var memberResponse = await _memberService.GetMemberByIdAsync(memberId);
                if (memberResponse.ISHaveErrorOrnNot)
                {
                    TempData["Error"] = "Member not found";
                    return RedirectToAction("MyMembers");
                }

                // Get available workout plans
                var workoutPlansResponse = await _workoutPlanService.GetAllWorkoutPlansAsync();

                ViewBag.Member = memberResponse.Result;
                ViewBag.WorkoutPlans = workoutPlansResponse.Result ?? new List<WorkoutPlanVM>();
                TempData["Error"] = response.ErrorMessage;
                return View(model);
            }

            TempData["Success"] = "Diet plan assigned successfully!";
            return RedirectToAction("MemberDetails", new { id = memberId });
        }






        // Edit Diet Plan Assignment
        [HttpGet]
        public async Task<IActionResult> EditDietPlan(int assignmentId, string memberId)
        {
            if (string.IsNullOrEmpty(memberId))
                return NotFound();

            var assignmentResponse = await _dietPlanAssignmentService.GetByIdAsync(assignmentId);
            if (assignmentResponse.ISHaveErrorOrnNot)
            {
                TempData["Error"] = "Diet assignment not found";
                return RedirectToAction("MemberDetails", new { id = memberId });
            }

            var memberResponse = await _memberService.GetMemberByIdAsync(memberId);
            var dietPlansResponse = await _dietPlanService.GetAllDietPlansAsync();

            ViewBag.Member = memberResponse.Result;
            ViewBag.MemberId = memberId;
            ViewBag.DietPlans = dietPlansResponse.Result ?? new List<DietPlanVM>();

            return View(assignmentResponse.Result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDietPlan(DietPlanAssignmentVM model, string memberId)
        {
            if (!ModelState.IsValid)
            {
                var memberResponse = await _memberService.GetMemberByIdAsync(memberId);
                var dietPlansResponse = await _dietPlanService.GetAllDietPlansAsync();
                ViewBag.Member = memberResponse.Result;
                ViewBag.MemberId = memberId;
                ViewBag.DietPlans = dietPlansResponse.Result ?? new List<DietPlanVM>();
                return View(model);
            }

            var response = await _dietPlanAssignmentService.UpdateAsync(model);

            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                var memberResponse = await _memberService.GetMemberByIdAsync(memberId);
                var dietPlansResponse = await _dietPlanService.GetAllDietPlansAsync();
                ViewBag.Member = memberResponse.Result;
                ViewBag.MemberId = memberId;
                ViewBag.DietPlans = dietPlansResponse.Result ?? new List<DietPlanVM>();
                return View(model);
            }

            TempData["Success"] = "Diet plan updated successfully!";
            return RedirectToAction("MemberDetails", new { id = memberId });
        }

        #endregion
    }
}
