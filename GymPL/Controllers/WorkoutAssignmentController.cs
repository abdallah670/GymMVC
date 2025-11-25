using GymBLL.ModelVM.Workout;
using GymBLL.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace GymPL.Controllers
{
    [Authorize(Roles = "Admin,Trainer")]
    public class WorkoutAssignmentController : Controller
    {
        private readonly IWorkoutAssignmentService _workoutAssignmentService;
        private readonly IMemberService _memberService;
        private readonly IWorkoutPlanService _workoutPlanService;

        public WorkoutAssignmentController(IWorkoutAssignmentService workoutAssignmentService, IMemberService memberService, IWorkoutPlanService workoutPlanService)
        {
            _workoutAssignmentService = workoutAssignmentService;
            _memberService = memberService;
            _workoutPlanService = workoutPlanService;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _workoutAssignmentService.GetAllAsync();
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return View(new List<WorkoutAssignmentVM>());
            }
            return View(response.Result);
        }

        public async Task<IActionResult> Details(int id)
        {
            var response = await _workoutAssignmentService.GetByIdAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return RedirectToAction(nameof(Index));
            }
            return View(response.Result);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateDropDowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorkoutAssignmentVM model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDowns();
                return View(model);
            }

            var response = await _workoutAssignmentService.CreateAsync(model);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                await PopulateDropDowns();
                return View(model);
            }

            TempData["Success"] = "Workout Plan assigned successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _workoutAssignmentService.GetByIdAsync(id);
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
        public async Task<IActionResult> Edit(WorkoutAssignmentVM model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDowns();
                return View(model);
            }

            var response = await _workoutAssignmentService.UpdateAsync(model);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                await PopulateDropDowns();
                return View(model);
            }

            TempData["Success"] = "Assignment updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _workoutAssignmentService.DeleteAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
            }
            else
            {
                TempData["Success"] = "Assignment deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var response = await _workoutAssignmentService.ToggleStatusAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                return Json(new { success = false, message = response.ErrorMessage });
            }
            return Json(new { success = true, message = "Status toggled successfully!" });
        }

        private async Task PopulateDropDowns()
        {
            var members = await _memberService.GetAllMembersAsync();
            var plans = await _workoutPlanService.GetActiveWorkoutPlansAsync();

            ViewBag.Members = new SelectList(members.Result ?? new List<GymBLL.ModelVM.User.Member.MemberVM>(), "Id", "FullName");
            ViewBag.WorkoutPlans = new SelectList(plans.Result ?? new List<WorkoutPlanVM>(), "Id", "PlanName");
        }
    }
}
