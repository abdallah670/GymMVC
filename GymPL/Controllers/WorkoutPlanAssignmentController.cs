using GymBLL.ModelVM;
using GymBLL.ModelVM.Member;
using GymBLL.ModelVM.Workout;
using GymBLL.Service.Abstract.Financial;
using GymBLL.Service.Abstract.Member;
using GymBLL.Service.Abstract.Workout;
using GymDAL.Repo.Abstract;
using GymPL.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymPL.Controllers
{
  
    public class WorkoutPlanAssignmentController : Controller
    {
        private readonly IWorkoutAssignmentService _workoutAssignmentService;
        private readonly IWorkoutPlanService _workoutPlanService;
        private readonly IMemberService _memberService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IWorkoutPlanItemService _workoutPlanItemService;
        private readonly IWorkoutLogService _workoutLogService;
        private readonly IUnitOfWork _unitOfWork;

        public WorkoutPlanAssignmentController(
            IWorkoutAssignmentService workoutAssignmentService,
            IWorkoutPlanService workoutPlanService,
            IMemberService memberService,
            ISubscriptionService subscriptionService,
          
            IWorkoutPlanItemService workoutPlanItemService,
            IWorkoutLogService workoutLogService,
            IUnitOfWork unitOfWork)
        {
            _workoutAssignmentService = workoutAssignmentService;
            _workoutPlanService = workoutPlanService;
            _memberService = memberService;
            _subscriptionService = subscriptionService;
            _workoutPlanItemService = workoutPlanItemService;
            _workoutLogService = workoutLogService;
            _unitOfWork = unitOfWork;
        }
        [Authorize(Roles = "Admin,Trainer")]
        [HttpGet]
        public async Task<IActionResult> Assign(string memberId, string returnUrl = null)
        {
            if (string.IsNullOrEmpty(memberId)) return NotFound();

            var memberResponse = await _memberService.GetMemberByIdAsync(memberId);
            if (memberResponse.ISHaveErrorOrnNot)
            {
                TempData["Error"] = "Member not found";
                return RedirectToReturnUrl(returnUrl);
            }

            var workoutPlansResponse = await _workoutPlanService.GetAllWorkoutPlansAsync();

            ViewBag.Member = memberResponse.Result;
            ViewBag.WorkoutPlans = workoutPlansResponse.Result ?? new List<WorkoutPlanVM>();
            ViewBag.ReturnUrl = returnUrl;

            var model = new WorkoutAssignmentVM
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1),
                IsActive = true
            };

            return View(model);
        }
        [Authorize(Roles = "Admin,Trainer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(WorkoutAssignmentVM model, string memberId, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var response = await _workoutAssignmentService.CreateAsync(model);
                if (!response.ISHaveErrorOrnNot)
                {
                    // Update Subscription with new Assignment
                    var subscriptionResponse = await _subscriptionService.GetByMemeberIdAsync(memberId);
                    
                    if (subscriptionResponse.Result == null)
                    {
                        // Member has no active subscription - cannot assign plan
                        TempData["Error"] = "Member does not have an active subscription. Please create a subscription first.";
                        return RedirectToReturnUrl(returnUrl, memberId);
                    }
                    
                    // Create a VM with just the ID to update the relationship
                    subscriptionResponse.Result.WorkoutAssignmentVM = new WorkoutAssignmentVM { Id = response.Result.Id };

                    await _subscriptionService.UpdateAsync(subscriptionResponse.Result);

                    TempData["Success"] = "Workout plan assigned successfully!";
                    return RedirectToReturnUrl(returnUrl, memberId);
                }
                TempData["Error"] = response.ErrorMessage;
            }

            // Reload data for view
            var memberResponse = await _memberService.GetMemberByIdAsync(memberId);
            ViewBag.Member = memberResponse.Result;
            var workoutPlansResponse = await _workoutPlanService.GetAllWorkoutPlansAsync();
            ViewBag.WorkoutPlans = workoutPlansResponse.Result ?? new List<WorkoutPlanVM>();
            ViewBag.ReturnUrl = returnUrl;

            return View(model);
        }
        [Authorize(Roles = "Admin,Trainer")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id, string returnUrl = null)
        {
            var assignmentResponse = await _workoutAssignmentService.GetByIdAsync(id);
            if (assignmentResponse.ISHaveErrorOrnNot)
            {
                TempData["Error"] = "Workout assignment not found";
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
            var workoutPlansResponse = await _workoutPlanService.GetAllWorkoutPlansAsync();

            ViewBag.Member = memberResponse.Result;
            ViewBag.MemberId = memberId;
            ViewBag.WorkoutPlans = workoutPlansResponse.Result ?? new List<WorkoutPlanVM>();
            ViewBag.ReturnUrl = returnUrl;

            return View(assignmentResponse.Result);
        }
        [Authorize(Roles = "Admin,Trainer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(WorkoutAssignmentVM model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var response = await _workoutAssignmentService.UpdateAsync(model);

                if (!response.ISHaveErrorOrnNot)
                {
                    TempData["Success"] = "Workout plan updated successfully!";
                    
                    // Get member ID from assignment
                    string memberId = await GetMemberIdFromAssignment(model.Id);
                    return RedirectToReturnUrl(returnUrl, memberId);
                }

                TempData["Error"] = response.ErrorMessage;
            }

            // Get member ID from assignment
            string memberIdForView = await GetMemberIdFromAssignment(model.Id);
            var memberResponse = await _memberService.GetMemberByIdAsync(memberIdForView);
            var workoutPlansResponse = await _workoutPlanService.GetAllWorkoutPlansAsync();
            
            ViewBag.Member = memberResponse.Result;
            ViewBag.MemberId = memberIdForView;
            ViewBag.WorkoutPlans = workoutPlansResponse.Result ?? new List<WorkoutPlanVM>();
            ViewBag.ReturnUrl = returnUrl;
            
            return View(model);
        }
        
        [Authorize(Roles = "Admin,Trainer")]
        [HttpGet]
        public async Task<IActionResult> Details(int id, string returnUrl = null)
        {
            var assignmentResponse = await _workoutAssignmentService.GetByIdAsync(id);
            if (assignmentResponse.ISHaveErrorOrnNot)
            {
                TempData["Error"] = "Workout assignment not found";
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
            // Query subscriptions by WorkoutAssignmentId using UnitOfWork
            var subscriptions = await _unitOfWork.Subscriptions.FindAsync(s => s.WorkoutAssignmentId == assignmentId);
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
        [Authorize(Roles = "Admin,Trainer,Member")]
        public async Task<IActionResult> ViewTodayWorkout(int? assignmentId = null, int? day = null)
        {
            string memberId;
            
            // If called by admin/trainer with assignmentId, get memberId from assignment
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
                // Member viewing their own workout plan
                memberId = User.GetUserId();
            }

            var subscriptionResponse = await _subscriptionService.GetSubscritionWorkoutByMemberIdAsync(memberId);
            
            if (subscriptionResponse.ISHaveErrorOrnNot || 
                subscriptionResponse.Result?.WorkoutAssignmentVM == null || 
                !subscriptionResponse.Result.WorkoutAssignmentVM.IsActive)
            {
                TempData["Error"] = "No active workout plan found.";
                if (User.IsMember())
                {
                    return RedirectToAction("Dashboard", "Member");
                }
                return RedirectToAction("Details", "Member", new { id = memberId });
            }

            var assignment = subscriptionResponse.Result.WorkoutAssignmentVM;
            var workoutPlanId = assignment.WorkoutPlanId;
            

            // Calculate Day Number (1-7) based on StartDate
            // DayOfWeek: Sunday=0, Monday=1... Saturday=6. 
            // Map to 1-7: Sunday=7, Monday=1, ... Saturday=6

            int dayNumber;
            if (day.HasValue && day.Value >= 1 && day.Value <= 7)
            {
                dayNumber = day.Value;
            }
            else
            {
                // Convert DayOfWeek to 1-7 (Sunday=1, Monday=2, ..., Saturday=7)
                dayNumber = (int)DateTime.Now.DayOfWeek+1;
               
            }

            var allItemsResponse = await _workoutPlanItemService.GetByWorkoutPlanIdAsync(workoutPlanId);
            
            if (allItemsResponse.ISHaveErrorOrnNot)
            {
                TempData["Error"] = "Unable to load workout details.";
                if (User.IsMember())
                {
                    return RedirectToAction("Dashboard", "Member");
                }
                return RedirectToAction("Details", "Member", new { id = memberId });
            }

            var todayItems = allItemsResponse.Result
                .Where(i => i.DayNumber == dayNumber && i.IsActive)
                .OrderBy(i => i.Id)
                .ToList();

            ViewBag.Assignment = assignment;
            ViewBag.WorkoutPlan = assignment.WorkoutPlan;
            ViewBag.CurrentDay = dayNumber;
            ViewBag.MaxDays = 7; // Weekly cycle
            ViewBag.Exercises = todayItems;
            ViewBag.PlanName = assignment.WorkoutPlan?.Name;
            ViewBag.DayName = DateTime.Now.DayOfWeek.ToString();
            ViewBag.Date = DateTime.Now.ToString("D");

            // Check if logged
            var logResponse = await _workoutLogService.GetMemberHistoryAsync(memberId);
            bool isLogged = false;
            if (!logResponse.ISHaveErrorOrnNot && logResponse.Result != null)
            {
                 // Assuming history is a list of logs. Check if any log matches today's date (or the viewed day's date relative to start date?)
                 // Simplification: Check if any log matches DateTime.Today. 
                 // Ideally, we should check if a log exists for this specific 'dayNumber' of the plan iteration.
                 // But for now, let's check if there's a log with Date.Date == DateTime.UtcNow.Date AND we are viewing the current day?
                 // Or just check if the user logged *anything* today.
                 
                 // If the user navigates to previous days, we need to know if THAT day was logged.
                 // This requires more complex logic linking LOG -> PLAN DAY.
                 // Current LogVM doesn't seem to store "PlanDayNumber". It stores Date.
                 
                 // Let's rely on Date. If viewing 'Today' (calculated from start date matching today), check if Log.Date == Today.
                 // If viewing 'Yesterday', check Log.Date == Yesterday.
                 
                 // However, 'day' param is just an integer (1-7).
                 // We need to calculate the actual DATE represented by that day number.
                 // StartDate + (Weeks passed * 7) + (DayNum - 1).
                 // This is getting complicated.
                 
                 // Simpler approach: Check if ANY log entry matches the current Date being viewed.
                 // But Member viewing "Day 1" might mean "Next Monday".
                 
                 // Let's stick to: Has the user logged a workout TODAY? And is the view showing TODAY?
                 if (dayNumber == (int)DateTime.Now.DayOfWeek + 1) // Assuming this mapping
                 {
                     isLogged = logResponse.Result.Any(l => l.Date.Date == DateTime.UtcNow.Date);
                 }
            }
            ViewBag.IsLogged = isLogged;

            return View(todayItems);
        }
    }
}

