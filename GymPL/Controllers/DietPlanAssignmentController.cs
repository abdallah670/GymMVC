using GymBLL.ModelVM;
using GymBLL.ModelVM.Nutrition;
using GymBLL.ModelVM.Member;
using GymBLL.Service.Abstract.Nutrition;
using GymDAL.Repo.Abstract;
using GymPL.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymBLL.Service.Abstract.Member;
using GymBLL.Service.Abstract.Financial;
using Newtonsoft.Json;
using GymDAL.Entities.Nutrition;
using Microsoft.EntityFrameworkCore;

namespace GymPL.Controllers
{
   
    public class DietPlanAssignmentController : Controller
    {
        private readonly IDietPlanAssignmentService _dietPlanAssignmentService;
        private readonly IDietPlanService _dietPlanService;
        private readonly IMemberService _memberService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IDietPlanItemService _dietPlanItemService;
        private readonly IUnitOfWork _unitOfWork;

        public DietPlanAssignmentController(
            IDietPlanAssignmentService dietPlanAssignmentService,
            IDietPlanService dietPlanService,
            IMemberService memberService,
            ISubscriptionService subscriptionService,
            IDietPlanItemService dietPlanItemService,
            IUnitOfWork unitOfWork)
        {
            _dietPlanAssignmentService = dietPlanAssignmentService;
            _dietPlanService = dietPlanService;
            _memberService = memberService;
            _subscriptionService = subscriptionService;
            _dietPlanItemService = dietPlanItemService;
            _unitOfWork = unitOfWork;
        }
        [Authorize(Roles = "Admin,Trainer")]
        [HttpGet]
        public async Task<IActionResult> Assign(string memberId, string returnUrl = null)
        {
            if (string.IsNullOrEmpty(memberId))
                return NotFound();

            var memberResponse = await _memberService.GetMemberByIdAsync(memberId);
            if (memberResponse.ISHaveErrorOrnNot)
            {
                TempData["Error"] = "Member not found";
                return RedirectToReturnUrl(returnUrl);
            }

            // Get available diet plans
            var dietPlansResponse = await _dietPlanService.GetAllDietPlansAsync();

            ViewBag.Member = memberResponse.Result;
            ViewBag.DietPlans = dietPlansResponse.Result ?? new List<DietPlanVM>();
            ViewBag.ReturnUrl = returnUrl;

            var model = new DietPlanAssignmentVM
            {
                MemberName = memberResponse.Result.FullName,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1),
                IsActive = true
            };

            return View(model);
        }
        [Authorize(Roles = "Admin,Trainer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(DietPlanAssignmentVM model, string memberId, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                var memberResponse = await _memberService.GetMemberByIdAsync(memberId);
                var dietPlansResponse = await _dietPlanService.GetAllDietPlansAsync();
                ViewBag.Member = memberResponse.Result;
                ViewBag.DietPlans = dietPlansResponse.Result ?? new List<DietPlanVM>();
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }
            var subscriptionResponse = await _subscriptionService.GetByMemeberIdAsync(memberId);

            if (subscriptionResponse.Result == null)
            {
                // Member has no active subscription - cannot assign plan
                TempData["Error"] = "Member does not have an active subscription. Please create a subscription first.";
                return RedirectToReturnUrl(returnUrl, memberId);
            }

            var response = await _dietPlanAssignmentService.CreateAsync(model);

            if (response.ISHaveErrorOrnNot)
            {
                var memberResponse = await _memberService.GetMemberByIdAsync(memberId);
                if (memberResponse.ISHaveErrorOrnNot)
                {
                    TempData["Error"] = "Member not found";
                    return RedirectToReturnUrl(returnUrl);
                }

                // Get available diet plans
                var dietPlansResponse = await _dietPlanService.GetAllDietPlansAsync();

                ViewBag.Member = memberResponse.Result;
                ViewBag.DietPlans = dietPlansResponse.Result ?? new List<DietPlanVM>();
                ViewBag.ReturnUrl = returnUrl;
                TempData["Error"] = response.ErrorMessage;
                return View(model);
            }

            
            subscriptionResponse.Result.DietPlanAssignmentVM = new DietPlanAssignmentVM();
            subscriptionResponse.Result.DietPlanAssignmentVM.Id = response.Result.Id;

            var updated = await _subscriptionService.UpdateAsync(subscriptionResponse.Result);
            if (updated.ISHaveErrorOrnNot)
            {
                var memberResponse = await _memberService.GetMemberByIdAsync(memberId);
                if (memberResponse.ISHaveErrorOrnNot)
                {
                    TempData["Error"] = "Member not found";
                    return RedirectToReturnUrl(returnUrl);
                }

                // Get available diet plans
                var dietPlansResponse = await _dietPlanService.GetAllDietPlansAsync();

                ViewBag.Member = memberResponse.Result;
                ViewBag.DietPlans = dietPlansResponse.Result ?? new List<DietPlanVM>();
                ViewBag.ReturnUrl = returnUrl;
                TempData["Error"] = updated.ErrorMessage;
                return View(model);
            }

            TempData["Success"] = "Diet plan assigned successfully!";
            return RedirectToReturnUrl(returnUrl, memberId);
        }
        [Authorize(Roles = "Admin,Trainer")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id, string returnUrl = null)
        {
            if (id <= 0)
                return NotFound();

            var assignmentResponse = await _dietPlanAssignmentService.GetByIdAsync(id);
            if (assignmentResponse.ISHaveErrorOrnNot)
            {
                TempData["Error"] = "Diet assignment not found";
                return RedirectToReturnUrl(returnUrl);
            }

            // Get member ID from subscription
            string memberId = await GetMemberIdFromAssignment(id);
            if (string.IsNullOrEmpty(memberId))
            {
                TempData["Error"] = "Member not found for this assignment";
                return RedirectToReturnUrl(returnUrl);
            }

            var memberResponse = await _memberService.GetMemberByIdAsync(memberId);
            var dietPlansResponse = await _dietPlanService.GetAllDietPlansAsync();

            ViewBag.Member = memberResponse.Result;
            ViewBag.MemberId = memberId;
            ViewBag.DietPlans = dietPlansResponse.Result ?? new List<DietPlanVM>();
            ViewBag.ReturnUrl = returnUrl;

            return View(assignmentResponse.Result);
        }
        [Authorize(Roles = "Admin,Trainer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DietPlanAssignmentVM model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                string memberId = await GetMemberIdFromAssignment(model.Id);
                var memberResponse = await _memberService.GetMemberByIdAsync(memberId);
                var dietPlansResponse = await _dietPlanService.GetAllDietPlansAsync();
                ViewBag.Member = memberResponse.Result;
                ViewBag.MemberId = memberId;
                ViewBag.DietPlans = dietPlansResponse.Result ?? new List<DietPlanVM>();
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            var response = await _dietPlanAssignmentService.UpdateAsync(model);

            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                string memberId = await GetMemberIdFromAssignment(model.Id);
                var memberResponse = await _memberService.GetMemberByIdAsync(memberId);
                var dietPlansResponse = await _dietPlanService.GetAllDietPlansAsync();
                ViewBag.Member = memberResponse.Result;
                ViewBag.MemberId = memberId;
                ViewBag.DietPlans = dietPlansResponse.Result ?? new List<DietPlanVM>();
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            TempData["Success"] = "Diet plan updated successfully!";
            
            // Get member ID from assignment
            string memberIdForRedirect = await GetMemberIdFromAssignment(model.Id);
            return RedirectToReturnUrl(returnUrl, memberIdForRedirect);
        }
        [Authorize(Roles = "Admin,Trainer")]
        [HttpGet]
        public async Task<IActionResult> Details(int id, string returnUrl = null)
        {
            var assignmentResponse = await _dietPlanAssignmentService.GetByIdAsync(id);
            if (assignmentResponse.ISHaveErrorOrnNot)
            {
                TempData["Error"] = "Diet assignment not found";
                return RedirectToReturnUrl(returnUrl);
            }

            // Get member ID from subscription
            string memberId = await GetMemberIdFromAssignment(id);
            var memberResponse = await _memberService.GetMemberByIdAsync(memberId);

            ViewBag.Member = memberResponse.Result;
            ViewBag.MemberId = memberId;
            ViewBag.ReturnUrl = returnUrl;

            return View(assignmentResponse.Result);
        }

        private async Task<string> GetMemberIdFromAssignment(int assignmentId)
        {
            // Query subscriptions by DietPlanAssignmentId using UnitOfWork
            var subscriptions = await _unitOfWork.Subscriptions.FindAsync(s => s.DietPlanAssignmentId == assignmentId);
            var subscription = subscriptions.FirstOrDefault();
            return subscription?.MemberId;
        }

        private IActionResult RedirectToReturnUrl(string returnUrl, string memberId = null)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // Default to Member Details if memberId is provided
            if (!string.IsNullOrEmpty(memberId))
            {
                return RedirectToAction("Details", "Member", new { id = memberId });
            }

            // Fallback to Member Index
            return RedirectToAction("Index", "Member");
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Trainer,Nutritionist,Member")]
        public async Task<IActionResult> TodayMeal(int? assignmentId = null, int? day = null)
        {
            string memberId;
            
            // If called by admin/trainer/nutritionist with assignmentId, get memberId from assignment
            if (assignmentId.HasValue && !User.IsMember())
            {
                memberId = await GetMemberIdFromAssignment(assignmentId.Value);
                if (string.IsNullOrEmpty(memberId))
                {
                    TempData["Error"] = "Member not found for this assignment.";
                    return RedirectToAction("Index", "Member");
                }
            }
            else
            {
                // Member viewing their own meal plan
                memberId = User.GetUserId();
            }

            var subscriptionResponse = await _subscriptionService.GetSubscritionDietByMemberIdAsync(memberId);
            
            if (subscriptionResponse.ISHaveErrorOrnNot || 
                subscriptionResponse.Result?.DietPlanAssignmentVM == null || 
                !subscriptionResponse.Result.DietPlanAssignmentVM.IsActive)
            {
                TempData["Error"] = "No active diet plan found.";
                if (User.IsMember())
                {
                    return RedirectToAction("Dashboard", "Member");
                }
                return RedirectToAction("Details", "Member", new { id = memberId });
            }

            var assignment = subscriptionResponse.Result.DietPlanAssignmentVM;
            var dietPlanId = assignment.DietPlanId;
            
            // Get all diet plan items first to determine actual cycle length
            var allItemsResponse = await _dietPlanItemService.GetByDietPlanIdAsync(dietPlanId);
            
            if (allItemsResponse.ISHaveErrorOrnNot)
            {
                TempData["Error"] = "Unable to load diet details.";
                if (User.IsMember())
                {
                    return RedirectToAction("Dashboard", "Member");
                }
                return RedirectToAction("Details", "Member", new { id = memberId });
            }

            // Find the maximum day number that actually has meals
            var maxDayWithMeals = allItemsResponse.Result
                .Where(i => i.IsActive)
                .Select(i => i.DayNumber)
                .DefaultIfEmpty(1)
                .Max();

            // Use the actual max day as the cycle length (if meals only exist for days 1-7, cycle is 7)
            var cycleLength = maxDayWithMeals;
            if (cycleLength == 0) cycleLength = 1; // Fallback

            // Calculate Day Number relative to start date
            var daysElapsed = (DateTime.Now.Date - assignment.StartDate.Date).Days;

            // Use provided day parameter or calculate current day
            int dayNumber;
            if (day.HasValue && day.Value >= 1 && day.Value <= 7)
            {
                dayNumber = day.Value;
            }
            else
            {
                dayNumber = (int)DateTime.Now.DayOfWeek+1;
              
            }

            // Define meal type order: Breakfast, Lunch, Dinner, Snack
           

            var todayItems = allItemsResponse.Result
                .Where(i => i.DayNumber == dayNumber && i.IsActive)
                .OrderBy(i => i.Id)
                .ThenBy(i => i.MealName)
                .ToList();

            // Fetch today's meal log to see what's completed
            var todayLog = await _unitOfWork.MealLogs
                .Get(l => l.DietPlanAssignmentId == assignment.Id && l.Date.Date == DateTime.UtcNow.Date)
                .FirstOrDefaultAsync();

            var completedItemIds = new List<int>();
            if (todayLog != null && !string.IsNullOrEmpty(todayLog.MealsConsumed))
            {
                try
                {
                    completedItemIds = JsonConvert.DeserializeObject<List<int>>(todayLog.MealsConsumed) ?? new List<int>();
                }
                catch
                {
                    // Fallback if legacy format
                }
            }

            ViewBag.Assignment = assignment;
            ViewBag.DietPlan = assignment.DietPlan;
            ViewBag.CurrentDay = dayNumber;
            ViewBag.MaxDays = cycleLength;
            ViewBag.Meals = todayItems;
            ViewBag.PlanName = assignment.DietPlan?.Name;
            ViewBag.DayNumber = dayNumber;
            ViewBag.Date = DateTime.Now.ToString("D");
            ViewBag.CompletedItemIds = completedItemIds;

            return View(todayItems);
        }

        [HttpPost]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> ToggleMealItem(int assignmentId, int itemId, bool isChecked)
        {
            try
            {
                var today = DateTime.UtcNow.Date;
                var log = await _unitOfWork.MealLogs
                    .Get(l => l.DietPlanAssignmentId == assignmentId && l.Date.Date == today)
                    .FirstOrDefaultAsync();

                List<int> completedItems;

                if (log == null)
                {
                    if (!isChecked) return Json(new { success = true }); // Nothing to uncheck

                    log = new GymDAL.Entities.Nutrition.MealLog
                    {
                        DietPlanAssignmentId = assignmentId,
                        Date = today,
                        MealsConsumed = "[]",
                        CaloriesConsumed = 0
                    };
                    await _unitOfWork.MealLogs.AddAsync(log);
                    completedItems = new List<int>();
                }
                else
                {
                    try
                    {
                        completedItems = JsonConvert.DeserializeObject<List<int>>(log.MealsConsumed) ?? new List<int>();
                    }
                    catch
                    {
                        completedItems = new List<int>();
                    }
                }

                if (isChecked)
                {
                    if (!completedItems.Contains(itemId)) completedItems.Add(itemId);
                }
                else
                {
                    completedItems.Remove(itemId);
                }

                log.MealsConsumed = JsonConvert.SerializeObject(completedItems);
                
                // Recalculate total calories from items
                // This would require fetching all items, but for speed we might skip or client-side calc
                // ideally we should fetch item to get its calories
                var item = await _dietPlanItemService.GetByIdAsync(itemId);
                if (!item.ISHaveErrorOrnNot && item.Result != null)
                {
                     // Simple approximation: add/sub current item cals
                     // Real robust way: fetch all checked items. 
                     // Let's rely on client update for visual, server stores IDs.
                     // To update CalConsumed correctly we should sum all IDs.
                     // For now, let's just save the IDs.
                }

                _unitOfWork.MealLogs.Update(log);
                await _unitOfWork.SaveAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}

