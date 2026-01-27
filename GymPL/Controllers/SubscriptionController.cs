using GymBLL.ModelVM.Financial;
using GymBLL.Service.Abstract.Financial;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using GymBLL.Service.Abstract.Communication;

namespace GymPL.Controllers
{
    // Adjust roles as needed
    [Authorize(Roles = "Trainer,Admin,Manager")]
    public class SubscriptionController : Controller
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly INotificationService _notificationService;

        public SubscriptionController(ISubscriptionService subscriptionService, INotificationService notificationService)
        {
            _subscriptionService = subscriptionService;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Index(string status = "All")
        {
            var response = await _subscriptionService.GetAllAsync();
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return View(new List<SubscriptionVM>());
            }

            var subscriptions = response.Result;

            // Simple filtering
            if (status != "All")
            {
                subscriptions = subscriptions.Where(s => s.Status == status).ToList();
            }

            ViewBag.CurrentFilter = status;

            return View(subscriptions);
        }

        public async Task<IActionResult> Details(int id)
        {
            var response = await _subscriptionService.GetByIdAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return RedirectToAction(nameof(Index));
            }
            return View(response.Result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            // Get subscription details first to know who to notify
            var subResponse = await _subscriptionService.GetByIdAsync(id);
            if (subResponse.ISHaveErrorOrnNot)
            {
                TempData["Error"] = "Subscription not found.";
                return RedirectToAction(nameof(Index));
            }

            var response = await _subscriptionService.CancelSubscriptionAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
            }
            else
            {
                // Send notification to member
                var notification = new GymBLL.ModelVM.Communication.NotificationVM
                {
                    UserId = subResponse.Result.MemberId,
                    Type = "Warning",
                    Message = $"Your subscription to {subResponse.Result.MembershipType} has been cancelled by an administrator.",
                    Status = "Unread",
                    DeliveryMethod = "InApp",
                    SendTime = System.DateTime.UtcNow
                };
                await _notificationService.CreateAsync(notification);

                TempData["Success"] = "Subscription cancelled successfully.";
            }
            return RedirectToAction(nameof(Index));
        }
       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _subscriptionService.DeleteAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = "Failed to delete subscription.";
            }
            else
            {
                TempData["Success"] = "Subscription deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
