using GymBLL.Service.Abstract.Communication;
using GymBLL.ModelVM.Communication;
using GymPL.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GymPL.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUnread()
        {
            var userId = User.GetUserId();
            var response = await _notificationService.GetUnreadByUserIdAsync(userId);
            
            if (response.ISHaveErrorOrnNot)
            {
                // Log error
                return Json(new { success = false, message = "Failed to fetch notifications" });
            }

            return Json(new { success = true, notifications = response.Result });
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var response = await _notificationService.MarkAsReadAsync(id);
            return Json(new { success = !response.ISHaveErrorOrnNot });
        }
        
        [Authorize(Roles = "Trainer,Admin,Manager")]
        [HttpGet]
        public IActionResult Send(string userId, string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            var model = new NotificationVM
            {
                UserId = userId,
                // Removed default Type to force selection or let validation handle it
                DeliveryMethod = "InApp"
            };
            return View(model);
        }

        [Authorize(Roles = "Trainer,Admin,Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(NotificationVM model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            model.Status = "Unread";
            model.SendTime = DateTime.UtcNow;
            
            var response = await _notificationService.CreateAsync(model);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            TempData["Success"] = "Notification sent successfully!";

            // Redirection Logic
            if (User.IsInRole("Member"))
            {
                return RedirectToAction("Dashboard", "Member");
            }
            
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // Default fallback
            return RedirectToAction("Index", "Member"); 
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
             var userId = User.GetUserId();
             var response = await _notificationService.GetByUserIdAsync(userId);
             if(response.ISHaveErrorOrnNot)
             {
                 TempData["Error"] = "Unable to load notifications.";
                 return View(new List<NotificationVM>());
             }
             return View(response.Result.OrderByDescending(x=>x.SendTime).ToList());
        }
    }
}
