using GymDAL.Entities.Users;
using GymPL.Global;
using GymPL.Models;
using GymWeb.Extensions;
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

        public HomeController(ILogger<HomeController> logger,
                            IOptions<GymSettings> settings,
                            UserManager<ApplicationUser> userManager) // Add UserManager
        {
            _logger = logger;
            _settings = settings.Value;
            _userManager = userManager;
        }

        public  IActionResult Index()
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
            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
