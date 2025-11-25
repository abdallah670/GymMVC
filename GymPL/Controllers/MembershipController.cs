using GymBLL.ModelVM.External;
using GymBLL.ModelVM.Nutrition;
using GymBLL.ModelVM.Workout;
using GymBLL.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace GymPL.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MembershipController : Controller
    {
        private readonly IMembershipService _membershipService;
        private readonly IDietPlanService _dietPlanService;
        private readonly IWorkoutPlanService _workoutPlanService;

        public MembershipController(IMembershipService membershipService, IDietPlanService dietPlanService, IWorkoutPlanService workoutPlanService)
        {
            _membershipService = membershipService;
            _dietPlanService = dietPlanService;
            _workoutPlanService = workoutPlanService;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _membershipService.GetAllAsync();
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return View(new List<MembershipVM>());
            }
            return View(response.Result);
        }

        public async Task<IActionResult> Details(int id)
        {
            var response = await _membershipService.GetByIdAsync(id);
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
        public async Task<IActionResult> Create(MembershipVM model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDowns();
                return View(model);
            }

            var response = await _membershipService.CreateAsync(model);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                await PopulateDropDowns();
                return View(model);
            }

            TempData["Success"] = "Membership created successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _membershipService.GetByIdAsync(id);
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
        public async Task<IActionResult> Edit(MembershipVM model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDowns();
                return View(model);
            }

            var response = await _membershipService.UpdateAsync(model);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                await PopulateDropDowns();
                return View(model);
            }

            TempData["Success"] = "Membership updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _membershipService.DeleteAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
            }
            else
            {
                TempData["Success"] = "Membership deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var response = await _membershipService.ToggleStatusAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                return Json(new { success = false, message = response.ErrorMessage });
            }
            return Json(new { success = true, message = "Status toggled successfully!" });
        }

        private async Task PopulateDropDowns()
        {
            var dietPlans = await _dietPlanService.GetActiveDietPlansAsync();
            var workoutPlans = await _workoutPlanService.GetActiveWorkoutPlansAsync();

            ViewBag.DietPlans = new SelectList(dietPlans.Result ?? new List<DietPlanVM>(), "Id", "PlanName");
            ViewBag.WorkoutPlans = new SelectList(workoutPlans.Result ?? new List<WorkoutPlanVM>(), "Id", "PlanName");
        }
    }
}
