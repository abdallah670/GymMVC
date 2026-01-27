using GymBLL.ModelVM.Workout;
using GymBLL.Service.Abstract.Workout;
using GymDAL.Entities.Workout;
using GymBLL.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace GymPL.Controllers
{
    [Authorize(Roles = "Trainer")]
    public class WorkoutPlanItemController : Controller
    {
        private readonly IWorkoutPlanItemService _workoutPlanItemService;
        private readonly IWorkoutPlanService _workoutPlanService;

        public WorkoutPlanItemController(IWorkoutPlanItemService workoutPlanItemService, IWorkoutPlanService workoutPlanService)
        {
            _workoutPlanItemService = workoutPlanItemService;
            _workoutPlanService = workoutPlanService;
        }

        public async Task<IActionResult> Index()
        {
            return RedirectToAction("Index", "WorkoutPlan");
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? workoutPlanId, string returnUrl = null)
        {
            await PopulateDropDowns();

            if (workoutPlanId.HasValue)
            {
                // Store returnUrl in ViewData to pass to the view
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["WorkoutPlanId"] = workoutPlanId.Value;

                var planResponse = await _workoutPlanService.GetWorkoutPlanNameAsync(workoutPlanId.Value);
                if (planResponse.ISHaveErrorOrnNot)
                {
                    TempData["Error"] = planResponse.ErrorMessage;
                }
                else
                {
                    ViewData["PlanName"] = planResponse.Result;
                }

                return View(new WorkoutPlanItemVM { WorkoutPlanId = workoutPlanId.Value });
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorkoutPlanItemVM model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDowns();
                ViewData["ReturnUrl"] = returnUrl;
                return View(model);
            }

            var response = await _workoutPlanItemService.CreateAsync(model);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                await PopulateDropDowns();
                ViewData["ReturnUrl"] = returnUrl;
                return View(model);
            }

            TempData["Success"] = "Exercise added successfully!";

            // If returnUrl is provided, redirect back to it
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // Otherwise redirect to workout plan details
            return RedirectToAction("Details", "Workout", new { id = model.WorkoutPlanId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, string returnUrl = null)
        {
            var response = await _workoutPlanItemService.GetByIdAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return RedirectToReturnUrl(returnUrl, response.Result?.WorkoutPlanId);
            }
            var planResponse = await _workoutPlanService.GetWorkoutPlanNameAsync(response.Result.WorkoutPlanId);
            if (planResponse.ISHaveErrorOrnNot)
            {
                TempData["Error"] = planResponse.ErrorMessage;
            }
            else
            {
                ViewData["PlanName"] = planResponse.Result;
            }

            // Store returnUrl in ViewData to pass to the view
            ViewData["ReturnUrl"] = returnUrl;

            await PopulateDropDowns();
            return View(response.Result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(WorkoutPlanItemVM model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDowns();
                ViewData["ReturnUrl"] = returnUrl;
                return View(model);
            }

            var response = await _workoutPlanItemService.UpdateAsync(model);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                await PopulateDropDowns();
                ViewData["ReturnUrl"] = returnUrl;
                return View(model);
            }

            TempData["Success"] = "Exercise updated successfully!";

            // If returnUrl is provided, redirect back to it
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // Otherwise redirect to workout plan details
            return RedirectToAction("Details", "WorkoutPlan", new { id = model.WorkoutPlanId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string returnUrl = null)
        {
            // First get the exercise to know the workout plan ID
            var exerciseResponse = await _workoutPlanItemService.GetByIdAsync(id);

            var response = await _workoutPlanItemService.DeleteAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
            }
            else
            {
                TempData["Success"] = "Exercise deleted successfully!";
            }

            // If returnUrl is provided, redirect back to it
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // Otherwise redirect to workout plan details
            var workoutPlanId = exerciseResponse.Result?.WorkoutPlanId ?? 0;
            if (workoutPlanId > 0)
            {
                return RedirectToAction("Details", "Workout", new { id = workoutPlanId });
            }

            return RedirectToAction("Index", "WorkoutPlan");
        }

        private async Task PopulateDropDowns()
        {
            var plans = await _workoutPlanService.GetAllWorkoutPlansAsync();
            ViewBag.WorkoutPlans = new SelectList(plans.Result ?? new List<WorkoutPlanVM>(), "Id", "Name");
        }

        private IActionResult RedirectToReturnUrl(string returnUrl, int? workoutPlanId = null)
        {
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            if (workoutPlanId.HasValue && workoutPlanId > 0)
            {
                return RedirectToAction("Details", "WorkoutPlan", new { id = workoutPlanId.Value });
            }

            return RedirectToAction("Index", "WorkoutPlan");
        }
    }
}
