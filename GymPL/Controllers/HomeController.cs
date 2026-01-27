using GymBLL.ModelVM.Trainer;
using GymBLL.Service.Abstract.Trainer;
using GymBLL.Service.Implementation.Member;
using GymDAL.Entities.Users;
using GymPL.Extensions;
using GymPL.Global;
using GymPL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace GymPL.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly GymSettings _settings;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly GymBLL.Service.Abstract.Financial.IMembershipService _membershipService;
        private readonly ITrainerReviewService _trainerReviewService;

        public HomeController(ILogger<HomeController> logger,
                            IOptions<GymSettings> settings,
                            UserManager<ApplicationUser> userManager,
                            GymBLL.Service.Abstract.Financial.IMembershipService membershipService,ITrainerReviewService trainerReviewService)
        {
            _logger = logger;
            _settings = settings.Value;
            _userManager = userManager;
            _membershipService = membershipService;
            _trainerReviewService= trainerReviewService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.HeaderType = "Public";
            if (_settings.IsFirstRun)
            {

                return RedirectToAction("CreateOwner", "Trainer");
            }
            else if(User.IsInRole("Trainer")&&User.Identity.IsAuthenticated)
            {
                ViewBag.HeaderType = "Trainer";
                ViewBag.Message = $"Welcome,{User.GetUserFullName} !";
                return RedirectToAction("Dashboard", "Trainer");

            }
            else if(User.IsInRole("Member") && User.Identity.IsAuthenticated)
            {
              
               
                ViewBag.HeaderType = "Member";
                return RedirectToAction("Dashboard", "Member");
            }
            var response = await _membershipService.GetActiveAsync();
            var Reviews=await _trainerReviewService.GetTop3ReviewsAsync();
            ViewBag.Memberships = response.ISHaveErrorOrnNot ? new List<GymBLL.ModelVM.Financial.MembershipVM>() : response.Result;
            ViewBag.Reviews = Reviews.ISHaveErrorOrnNot ? new List<TrainerReviewVM>() : Reviews.Result;
            ViewBag.ReviewsCount=Reviews.Result.Count;



            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Help()
        {
            ViewBag.Email=User.GetUserEmail();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
