using GymBLL.ModelVM.Nutrition;
using GymBLL.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymPL.Controllers
{
    [Authorize(Roles = "Admin,Trainer,Nutritionist")]
    public class DietPlanController : Controller
    {
        private readonly IDietPlanService _dietPlanService;

        public DietPlanController(IDietPlanService dietPlanService)
        {
            _dietPlanService = dietPlanService;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _dietPlanService.GetAllDietPlansAsync();
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return View(new List<DietPlanVM>());
            }
            return View(response.Result);
        }

        public async Task<IActionResult> Details(int id)
        {
            var response = await _dietPlanService.GetDietPlanByIdAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return RedirectToAction(nameof(Index));
            }
            return View(response.Result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DietPlanVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var response = await _dietPlanService.CreateDietPlanAsync(model);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return View(model);
            }

            TempData["Success"] = "Diet Plan created successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _dietPlanService.GetDietPlanByIdAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return RedirectToAction(nameof(Index));
            }
            return View(response.Result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DietPlanVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var response = await _dietPlanService.UpdateDietPlanAsync(model);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return View(model);
            }

            TempData["Success"] = "Diet Plan updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _dietPlanService.DeleteDietPlanAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
            }
            else
            {
                TempData["Success"] = "Diet Plan deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var response = await _dietPlanService.ToggleStatusAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                return Json(new { success = false, message = response.ErrorMessage });
            }
            return Json(new { success = true, message = "Status toggled successfully!" });
        }
    }
}
