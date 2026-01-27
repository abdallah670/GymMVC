using GymBLL.ModelVM.Workout;
using GymBLL.Service.Abstract.Workout;
using GymBLL.Service.Abstract.Financial;
using GymPL.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GymPL.Controllers
{
    [Authorize]
    public class WorkoutLogController : Controller
    {
        private readonly IWorkoutLogService _workoutLogService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IWorkoutPlanItemService _workoutPlanItemService;

        public WorkoutLogController(
            IWorkoutLogService workoutLogService,
            ISubscriptionService subscriptionService,
            IWorkoutPlanItemService workoutPlanItemService)
        {
            _workoutLogService = workoutLogService;
            _subscriptionService = subscriptionService;
            _workoutPlanItemService = workoutPlanItemService;
        }

        [HttpGet]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> LogWorkout(int? day = null)
        {
            var memberId = User.GetUserId();
            
            // Get member's active workout assignment
            var subscriptionResponse = await _subscriptionService.GetSubscritionWorkoutByMemberIdAsync(memberId);
            
            if (subscriptionResponse.ISHaveErrorOrnNot || 
                subscriptionResponse.Result?.WorkoutAssignmentVM == null ||
                !subscriptionResponse.Result.WorkoutAssignmentVM.IsActive)
            {
                TempData["Error"] = "No active workout plan found.";
                return RedirectToAction("Dashboard", "Member");
            }

            var assignment = subscriptionResponse.Result.WorkoutAssignmentVM;
            var workoutPlanId = assignment.WorkoutPlanId;

            // Calculate day number (1-7)
            int dayNumber = day.HasValue && day.Value >= 1 && day.Value <= 7 
                ? day.Value 
                : (int)DateTime.Now.DayOfWeek + 1;

            // Get exercises for the day
            var itemsResponse = await _workoutPlanItemService.GetByWorkoutPlanIdAsync(workoutPlanId);
            
            if (itemsResponse.ISHaveErrorOrnNot)
            {
                TempData["Error"] = "Unable to load workout details.";
                return RedirectToAction("Dashboard", "Member");
            }

            var todayItems = itemsResponse.Result
                .Where(i => i.DayNumber == dayNumber && i.IsActive)
                .OrderBy(i => i.Id)
                .ToList();

            if (!todayItems.Any())
            {
                TempData["Info"] = "No exercises for this day. It's a rest day!";
                return RedirectToAction("ViewTodayWorkout", "WorkoutPlanAssignment", new { day = dayNumber });
            }

            // Build the log model with pre-filled entries from plan defaults
            var logModel = new WorkoutLogVM
            {
                WorkoutPlanId = workoutPlanId,
                WorkoutPlanName = assignment.WorkoutPlan?.Name ?? "Workout",
                Date = DateTime.UtcNow,
                Entries = todayItems.Select(item => new WorkoutLogEntryVM
                {
                    WorkoutPlanItemId = item.Id,
                    ExerciseName = item.ExerciseName,
                    SetsPerformed = ParseSets(item.Sets),
                    RepsPerformed = item.Reps ?? "",
                    WeightLifted = "",
                    TargetSets = item.Sets,
                    TargetReps = item.Reps
                }).ToList()
            };

            ViewBag.DayNumber = dayNumber;
            ViewBag.DayName = ((DayOfWeek)(dayNumber == 7 ? 0 : dayNumber)).ToString();
            ViewBag.Assignment = assignment;

            return View(logModel);
        }

        private int ParseSets(string? sets)
        {
            if (string.IsNullOrEmpty(sets)) return 3; // Default
            if (int.TryParse(sets, out int result)) return result;
            return 3;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorkoutLogVM model)
        {
            var memberId = User.GetUserId();
            model.MemberId = memberId;

            var response = await _workoutLogService.LogWorkoutAsync(model);

            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return RedirectToAction("Dashboard", "Member");
            }

            TempData["Success"] = "High Five! Workout logged successfully. Great job! ✋";
            return RedirectToAction("History");
        }

        [HttpGet]
        public async Task<IActionResult> History()
        {
            var memberId = User.GetUserId();
            var response = await _workoutLogService.GetMemberHistoryAsync(memberId);

            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = "Could not load workout history.";
                return RedirectToAction("Dashboard", "Member");
            }

            return View(response.Result);
        }
    }
}

