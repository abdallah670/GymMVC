using GymBLL.ModelVM;
using GymBLL.ModelVM.Nutrition;
using GymBLL.Service.Abstract.Nutrition;
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

        public async Task<IActionResult> Index(int page = 1, int pageSize = 3)
        {
            if (page < 1) page = 1;
            var response = await _dietPlanService.GetPagedDietPlansAsync(page, pageSize);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return View(new PagedResult<DietPlanVM>());
            }
            return View(response.Result);
        }

        public async Task<IActionResult> Details(int id, string returnUrl = null, int returnPage = 1)
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
        public IActionResult Create(int returnPage = 1, string returnUrl = null)
        {
            var model = new DietPlanVM
            {
                DurationDays = 30, // Clear default so user must enter
                DietType = "" // Clear default so user must select
            };

            ViewData["ReturnPage"] = returnPage;
            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DietPlanVM model, int returnPage = 1, string returnUrl = null)
        {
            // Clear validation for DietPlanId in items - it will be set by the service
            foreach (var key in ModelState.Keys.Where(k => k.Contains("DietPlanItemsVM") && k.Contains("DietPlanId")).ToList())
            {
                ModelState.Remove(key);
            }

            if (!ModelState.IsValid)
            {
                ViewData["ReturnPage"] = returnPage;
                ViewData["ReturnUrl"] = returnUrl;
                return View(model);
            }

            var response = await _dietPlanService.CreateDietPlanAsync(model);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                ViewData["ReturnPage"] = returnPage;
                ViewData["ReturnUrl"] = returnUrl;
                return View(model);
            }

            TempData["Success"] = "Diet Plan created successfully!";
            return RedirectToAction(nameof(Index), new { page = returnPage });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, string returnUrl = null, int returnPage = 1)
        {
            var response = await _dietPlanService.GetDietPlanByIdAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return RedirectToAction(nameof(Index), new { page = returnPage });
            }
            // Pass both to view
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["ReturnPage"] = returnPage;
            return View(response.Result);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DietPlanVM model, string returnUrl = null, int returnPage = 1)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["ReturnPage"] = returnPage;
                return View(model);
            }
            var response = await _dietPlanService.UpdateDietPlanAsync(model);
            if (response.ISHaveErrorOrnNot)
            {
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["ReturnPage"] = returnPage;
                TempData["Error"] = response.ErrorMessage;
                return View(model);
            }

            TempData["Success"] = "Diet Plan updated successfully!";
            return RedirectToReturnUrl(returnUrl, returnPage);
        }
        // Universal redirect helper
        private IActionResult RedirectToReturnUrl(string returnUrl, int returnPage = 1)
        {
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // Default to Index with page number
            return RedirectToAction("Index", new { page = returnPage });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string returnUrl = null, int returnPage = 1)
        {
            var response = await _dietPlanService.DeleteDietPlanAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["ReturnPage"] = returnPage;
                TempData["Error"] = response.ErrorMessage;
                return RedirectToReturnUrl(returnUrl, returnPage);
            }
            
            TempData["Success"] = "Diet Plan deleted successfully!";
            
            // Check if the current page is now empty
            if (returnPage > 1)
            {
                var itemsOnPage = await _dietPlanService.GetPagedDietPlansAsync(returnPage, 3);
                if (itemsOnPage.Result.Items.Count == 0)
                {
                    returnPage--; // Redirect to previous page
                    // Don't use returnUrl as it contains the old page number
                    return RedirectToAction("Index", new { page = returnPage });
                }
            }
            
            return RedirectToReturnUrl(returnUrl, returnPage);
        }

       
    }
}
