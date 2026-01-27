using GymBLL.ModelVM;
using GymBLL.ModelVM.Workout;
using GymBLL.Service.Abstract.Workout;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymPL.Controllers
{
    [Authorize(Roles = "Admin,Trainer")]
    public class WorkoutPlanController : Controller
    {
        private readonly IWorkoutPlanService _workoutPlanService;

        public WorkoutPlanController(IWorkoutPlanService workoutPlanService)
        {
            _workoutPlanService = workoutPlanService;
        }

        public async Task<IActionResult> Index(int page=1,int pagesize=6)
        {
            if (page < 1) page = 1;
            var response = await _workoutPlanService.GetPagedWorkoutPlansAsync(page,pagesize);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return View(new PagedResult<WorkoutPlanVM>());
            }
          
            return View(response.Result);
        }

        public async Task<IActionResult> Details(int id,string returnUrl = null, int returnPage = 1)
        {
            var response = await _workoutPlanService.GetWorkoutPlanByIdAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return RedirectToReturnUrl(returnUrl,returnPage);
            }

            return View(response.Result);
        }

        [HttpGet]
        public IActionResult Create(int page = 1, string returnUrl = null)
        {
            ViewData["ReturnPage"] = page;
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorkoutPlanVM model, int page = 1, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ReturnPage"] = page;
                ViewData["ReturnUrl"] = returnUrl;
                return View(model);
            }

            var response = await _workoutPlanService.CreateWorkoutPlanAsync(model);
            if (response.ISHaveErrorOrnNot)
            {
                ViewData["ReturnPage"] = page;
                ViewData["ReturnUrl"] = returnUrl;
                TempData["Error"] = response.ErrorMessage;
                return View(model);
            }
            TempData["Success"] = "Workout Plan created successfully!";

            return RedirectToAction(nameof(Index), new { page = page });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, string returnUrl = null, int returnPage = 1)
        {
            var response = await _workoutPlanService.GetWorkoutPlanByIdAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return RedirectToAction(nameof(Index), new { page = returnPage });
            }

            // Pass returnPage to the view
            // Pass both to view
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["ReturnPage"] = returnPage;
            return View(response.Result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(WorkoutPlanVM model, string returnUrl = null, int returnPage = 1)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["ReturnPage"] = returnPage;
                return View(model);
            }

            var response = await _workoutPlanService.UpdateWorkoutPlanAsync(model);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                 ViewData["ReturnUrl"] = returnUrl;
               ViewData["ReturnPage"] = returnPage;
                return View(model);
            }

            TempData["Success"] = "Workout Plan updated successfully!";
            // Redirect appropriately
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
            var response = await _workoutPlanService.DeleteWorkoutPlanAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["ReturnPage"] = returnPage;
                TempData["Error"] = response.ErrorMessage;
            }
            else
            {
                TempData["Success"] = "Workout Plan deleted successfully!";
            }
            return RedirectToReturnUrl(returnUrl, returnPage);
        }

    }
}
