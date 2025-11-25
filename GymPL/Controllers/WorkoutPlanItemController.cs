using GymBLL.ModelVM.Workout;
using GymBLL.Service.Abstract;
using MenoBLL.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace GymPL.Controllers
{
    [Authorize(Roles = "Admin,Trainer")]
    public class WorkoutPlanItemController : Controller
    {
        private readonly IWorkoutPlanItemService _workoutPlanItemService;
        private readonly IWorkoutPlanService _workoutPlanService;

        public WorkoutPlanItemController(IWorkoutPlanItemService workoutPlanItemService, IWorkoutPlanService workoutPlanService)
        {
            _workoutPlanItemService = workoutPlanItemService;
            _workoutPlanService = workoutPlanService;
        }

        public async Task<IActionResult> Index(int? workoutPlanId)
        {
            Response<List<WorkoutPlanItemVM>> response;
            
            if (workoutPlanId.HasValue)
            {
                response = await _workoutPlanItemService.GetByWorkoutPlanIdAsync(workoutPlanId.Value);
                ViewBag.WorkoutPlanId = workoutPlanId;
            }
            else
            {
                response = await _workoutPlanItemService.GetAllAsync();
            }

            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return View(new List<WorkoutPlanItemVM>());
            }
            return View(response.Result);
        }

        public async Task<IActionResult> Details(int id)
        {
            var response = await _workoutPlanItemService.GetByIdAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return RedirectToAction(nameof(Index));
            }
            return View(response.Result);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? workoutPlanId)
        {
            await PopulateDropDowns();
            if (workoutPlanId.HasValue)
            {
                return View(new WorkoutPlanItemVM { WorkoutPlanId = workoutPlanId.Value });
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorkoutPlanItemVM model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDowns();
                return View(model);
            }

            var response = await _workoutPlanItemService.CreateAsync(model);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                await PopulateDropDowns();
                return View(model);
            }

            TempData["Success"] = "Exercise added successfully!";
            return RedirectToAction(nameof(Index), new { workoutPlanId = model.WorkoutPlanId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _workoutPlanItemService.GetByIdAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return RedirectToAction(nameof(Index));
            }
            await PopulateDropDowns();
            return View(response.Result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(WorkoutPlanItemVM model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDowns();
                return View(model);
            }

            var response = await _workoutPlanItemService.UpdateAsync(model);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                await PopulateDropDowns();
                return View(model);
            }

            TempData["Success"] = "Exercise updated successfully!";
            return RedirectToAction(nameof(Index), new { workoutPlanId = model.WorkoutPlanId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _workoutPlanItemService.DeleteAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
            }
            else
            {
                TempData["Success"] = "Exercise deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var response = await _workoutPlanItemService.ToggleStatusAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                return Json(new { success = false, message = response.ErrorMessage });
            }
            return Json(new { success = true, message = "Status toggled successfully!" });
        }

        private async Task PopulateDropDowns()
        {
            var plans = await _workoutPlanService.GetAllWorkoutPlansAsync();
            ViewBag.WorkoutPlans = new SelectList(plans.Result ?? new List<WorkoutPlanVM>(), "Id", "PlanName");
        }
    }
}
