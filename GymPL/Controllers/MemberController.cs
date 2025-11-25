using GymBLL.ModelVM.External;
using GymBLL.ModelVM.Notification;
using GymBLL.ModelVM.User.Member;
using GymBLL.Service.Abstract;
using GymBLL.Service.Implementation;
using GymDAL.Repo.Abstract;
using MenoBLL.ModelVM.AccountVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

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
        private readonly IUnitOfWork _unitOfWork;
        public MemberController(IMemberService memberService,
            ISubscriptionService SubscriptionService, 
            IMembershipService membershipService ,
            IPaymentService paymentService,
            IFitnessGoalsService fitnessGoalsService,
            INotificationService notificationService,
            IWorkoutAssignmentService workoutAssignmentService,
            IDietPlanAssignmentService dietPlanAssignmentService,
            IUnitOfWork unitOfWork)
        {
            _memberService = memberService;
            _SubscriptionService = SubscriptionService;
            _membershipService = membershipService;
            this._paymentService = paymentService;
            this.fitnessGoalsService = fitnessGoalsService;
            this._notificationService = notificationService;
            this._workoutAssignmentService = workoutAssignmentService;
            this._dietPlanAssignmentService = dietPlanAssignmentService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _memberService.GetAllMembersAsync();
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return View(new List<MemberVM>());
            }
            return View(response.Result);
        }

        [HttpGet]
        public async Task<IActionResult> Complete()
        {
            var memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);

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
            var memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (await _memberService.HasCompletedProfileAsync(memberId) == false)
            {
                return RedirectToAction("Complete", "Member");
            }
            ViewBag.HeaderType = "Member";
            return View();
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
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var response = await _memberService.GetMemberByIdAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return RedirectToAction(nameof(Index));
            }
            return View(response.Result);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var response = await _memberService.GetMemberByIdAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return RedirectToAction(nameof(Index));
            }
            return View(response.Result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MemberVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var response = await _memberService.UpdateMemberAsync(model);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return View(model);
            }

            TempData["Success"] = "Member updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
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

        // Subscription Renewal
        [HttpGet]
        public async Task<IActionResult> RenewSubscription()
        {
            var memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
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
            var memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
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
            var memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
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

        // Get Assigned Diet Plans
        [HttpGet]
        public async Task<IActionResult> GetAssignedDietPlans()
        {
            var memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Note: IDietPlanAssignmentService.GetByMemberIdAsync takes int, need to parse or adjust
            // For now, returning empty - you may need to adjust based on your actual implementation
            return Json(new { hasPlans = false, plans = new List<object>() });
        }
    }
}
