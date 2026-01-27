using GymBLL.ModelVM.Nutrition;
using GymBLL.Service.Abstract.Financial;
using GymBLL.Service.Abstract.Nutrition;
using GymPL.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymPL.Controllers
{
    [Authorize]
    public class MealLogController : Controller
    {
        private readonly IMealLogService _mealLogService;
        private readonly IDietPlanAssignmentService _dietPlanAssignmentService;
        private readonly ISubscriptionService _SubscriptionService;


        public MealLogController(IMealLogService mealLogService,ISubscriptionService subscriptionService, IDietPlanAssignmentService dietPlanAssignmentService)
        {
            _mealLogService = mealLogService;
            _dietPlanAssignmentService = dietPlanAssignmentService;
            _SubscriptionService = subscriptionService;
        }

        public async Task<IActionResult> Index()
        {
            // Ideally filter by User, but for now GetAll
            var dietplanassignment = await _SubscriptionService.GetAssignedDietPlan(User.GetUserId());
            var response = await _mealLogService.GetByDietPlanAssignmentIdAsync(dietplanassignment.Result);
           
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return View(new List<MealLogVM>());
            }
            return View(response.Result);
        }

        public async Task<IActionResult> Create(int assignmentId)
        {
           var dietPlanAssignmentResponse = await _dietPlanAssignmentService.GetLastWorkoutForMemberAsync(assignmentId);
            if (dietPlanAssignmentResponse.ISHaveErrorOrnNot)
            {
                TempData["Error"] = dietPlanAssignmentResponse.ErrorMessage;
                return RedirectToAction("TodayMeal","DietPlanAssignment");
            }
            ViewBag.DietPlanAssignment = dietPlanAssignmentResponse.Result;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MealLogVM model)
        {
            if (ModelState.IsValid)
            {
                var response = await _mealLogService.CreateAsync(model);
                if (response.ISHaveErrorOrnNot)
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        return Json(new { success = false, message = response.ErrorMessage });

                    TempData["Error"] = response.ErrorMessage;
                }
                else
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        return Json(new { success = true, message = "Meal Log created successfully." });

                    TempData["Success"] = "Meal Log created successfully.";
                    return RedirectToAction("Create",model.DietPlanAssignmentId);
                }
            }
            
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = false, message = "Invalid data provided." });

          
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _mealLogService.DeleteAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
            }
            else
            {
                TempData["Success"] = "Meal Log deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

       
    }
}
