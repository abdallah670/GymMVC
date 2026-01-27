using GymBLL.ModelVM.Member;
using GymBLL.Service.Abstract.Member;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymPL.Controllers
{
    [Authorize(Roles = "Admin,Manager,Trainer")]
    public class FitnessGoalsController : Controller
    {
        private readonly IFitnessGoalsService _fitnessGoalsService;

        public FitnessGoalsController(IFitnessGoalsService fitnessGoalsService)
        {
            _fitnessGoalsService = fitnessGoalsService;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _fitnessGoalsService.GetAllFitnessGoalsAsync();
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return View(new List<FitnessGoalsVM>());
            }
            return View(response.Result);
        }

        public async Task<IActionResult> Details(int id)
        {
            var response = await _fitnessGoalsService.GetByIdAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return RedirectToAction(nameof(Index));
            }
            return View(response.Result);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FitnessGoalsVM model)
        {
            if (ModelState.IsValid)
            {
                var response = await _fitnessGoalsService.CreateFitnessGoalAsync(model);
                if (response.ISHaveErrorOrnNot)
                {
                    TempData["Error"] = response.ErrorMessage;
                }
                else
                {
                    TempData["Success"] = "Fitness Goal created successfully.";
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var response = await _fitnessGoalsService.GetByIdAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return RedirectToAction(nameof(Index));
            }
            return View(response.Result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FitnessGoalsVM model)
        {
            if (ModelState.IsValid)
            {
                var response = await _fitnessGoalsService.UpdateFitnessGoalAsync(model);
                if (response.ISHaveErrorOrnNot)
                {
                    TempData["Error"] = response.ErrorMessage;
                }
                else
                {
                    TempData["Success"] = "Fitness Goal updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var response = await _fitnessGoalsService.GetByIdAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return RedirectToAction(nameof(Index));
            }
            return View(response.Result);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _fitnessGoalsService.DeleteFitnessGoalAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
            }
            else
            {
                TempData["Success"] = "Fitness Goal deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
