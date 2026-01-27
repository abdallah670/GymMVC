using GymPL.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using GymBLL.Service.Abstract.Trainer;
using GymBLL.ModelVM.Trainer;

namespace GymPL.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly ITrainerReviewService _reviewService;

        private readonly UserManager<GymDAL.Entities.Users.ApplicationUser> _userManager;

        public ReviewController(ITrainerReviewService reviewService, UserManager<GymDAL.Entities.Users.ApplicationUser> userManager)
        {
            _reviewService = reviewService;
            _userManager = userManager;
        }

        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Create()
        {
            var trainers = await _userManager.GetUsersInRoleAsync("Trainer");
            var model = new TrainerReviewVM();
           
          
            // Pass trainers to view via ViewBag for dropdown
            ViewBag.Trainers = trainers.Select(t => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem 
            { 
                Text = t.FullName, 
                Value = t.Id,
                Selected = trainers.Count == 1 // Auto-select if only one
            }).ToList();

            if (trainers.Count == 1)
            {
                model.TrainerId = trainers.First().Id;
            }
            ViewBag.MemberId= User.GetUserId();
            ViewBag.MemberPicture = User.GetProfilePicture();

            return PartialView("_CreateReviewModal", model);
        }

        [HttpPost]
        [Authorize(Roles = "Member")]
        // Remove ValidateAntiForgeryToken if relying strictly on AJAX/API calls or ensure token is passed. 
        // For Modal usage with standard form submit, it's safer to keep it.
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> Create(TrainerReviewVM model)
        {
           
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data provided." });
            }

            
            var response = await _reviewService.CreateReviewAsync(model, model.MemberId);

            if (response.ISHaveErrorOrnNot)
            {
                return Json(new { success = false, message = response.ErrorMessage });
            }

            return Json(new { success = true, message = "Review submitted successfully!" });
        }

        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> Index()
        {
            var trainerId = User.GetUserId();
            var response = await _reviewService.GetReviewsForTrainerAsync(trainerId);
            
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return RedirectToAction("Index", "Home");
            }
            

            return View(response.Result);
        }
    }
}
