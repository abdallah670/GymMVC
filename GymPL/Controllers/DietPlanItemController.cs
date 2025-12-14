using GymBLL.ModelVM.Nutrition;
using GymBLL.Service.Abstract;
using GymDAL.Entities.Nutrition;
using MenoBLL.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymPL.Controllers
{
    [Authorize(Roles = "Trainer,Nutritionist,Admin")]
    public class DietPlanItemController : Controller
    {
        private readonly IDietPlanItemService _dietPlanItemService;
        private readonly IDietPlanService _dietPlanService;

        public DietPlanItemController(IDietPlanItemService dietPlanItemService, IDietPlanService dietPlanService)
        {
            _dietPlanItemService = dietPlanItemService;
            _dietPlanService = dietPlanService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Index", "DietPlan");
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? dietPlanId, string returnUrl = null)
        {
            await PopulateDropDowns();

            if (dietPlanId.HasValue)
            {
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["DietPlanId"] = dietPlanId.Value;

                var planResponse = await _dietPlanService.GetDietPlanByIdAsync(dietPlanId.Value);
                if (planResponse.ISHaveErrorOrnNot)
                {
                    TempData["Error"] = planResponse.ErrorMessage;
                }
                else
                {
                    ViewData["PlanName"] = planResponse.Result.Name;
                }

                return View(new DietPlanItemVM { DietPlanId = dietPlanId.Value });
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DietPlanItemVM model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDowns();
                ViewData["ReturnUrl"] = returnUrl;
                return View(model);
            }

            var response = await _dietPlanItemService.CreateAsync(model);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                await PopulateDropDowns();
                ViewData["ReturnUrl"] = returnUrl;
                return View(model);
            }

            TempData["Success"] = "Meal added successfully!";

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Details", "DietPlan", new { id = model.DietPlanId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, string returnUrl = null)
        {
            var response = await _dietPlanItemService.GetByIdAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return RedirectToReturnUrl(returnUrl, response.Result?.DietPlanId);
            }
            
            var planResponse = await _dietPlanService.GetDietPlanByIdAsync(response.Result.DietPlanId);
            if (planResponse.ISHaveErrorOrnNot)
            {
                TempData["Error"] = planResponse.ErrorMessage;
            }
            else
            {
                ViewData["PlanName"] = planResponse.Result.Name;
            }

            ViewData["ReturnUrl"] = returnUrl;

            await PopulateDropDowns();
            return View(response.Result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DietPlanItemVM model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDowns();
                ViewData["ReturnUrl"] = returnUrl;
                return View(model);
            }

            var response = await _dietPlanItemService.UpdateAsync(model);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                await PopulateDropDowns();
                ViewData["ReturnUrl"] = returnUrl;
                return View(model);
            }

            TempData["Success"] = "Meal updated successfully!";

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Details", "DietPlan", new { id = model.DietPlanId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string returnUrl = null)
        {
            var itemResponse = await _dietPlanItemService.GetByIdAsync(id);
            var response = await _dietPlanItemService.DeleteAsync(id);
            
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
            }
            else
            {
                TempData["Success"] = "Meal deleted successfully!";
            }

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            var dietPlanId = itemResponse.Result?.DietPlanId ?? 0;
            if (dietPlanId > 0)
            {
                return RedirectToAction("Details", "DietPlan", new { id = dietPlanId });
            }

            return RedirectToAction("Index", "DietPlan");
        }

        private async Task PopulateDropDowns()
        {
            var plans = await _dietPlanService.GetAllDietPlansAsync();
            ViewBag.DietPlans = new SelectList(plans.Result ?? new List<DietPlanVM>(), "Id", "Name");
        }

        private IActionResult RedirectToReturnUrl(string returnUrl, int? dietPlanId = null)
        {
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            if (dietPlanId.HasValue && dietPlanId > 0)
            {
                return RedirectToAction("Details", "DietPlan", new { id = dietPlanId.Value });
            }

            return RedirectToAction("Index", "DietPlan");
        }
    }
}
