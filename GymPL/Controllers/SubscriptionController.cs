using GymBLL.ModelVM.External;
using GymBLL.ModelVM.User.Member;
using GymBLL.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace GymPL.Controllers
{
    [Authorize(Roles = "Admin,Trainer")]
    public class SubscriptionController : Controller
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly IMemberService _memberService;
        private readonly IMembershipService _membershipService;

        public SubscriptionController(ISubscriptionService subscriptionService, IMemberService memberService, IMembershipService membershipService)
        {
            _subscriptionService = subscriptionService;
            _memberService = memberService;
            _membershipService = membershipService;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _subscriptionService.GetAllAsync();
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return View(new List<SubscriptionVM>());
            }
            return View(response.Result);
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

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateDropDowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubscriptionVM model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDowns();
                return View(model);
            }

            var response = await _subscriptionService.CreateAsync(model);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                await PopulateDropDowns();
                return View(model);
            }

            TempData["Success"] = "Subscription created successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _subscriptionService.GetByIdAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return RedirectToAction(nameof(Index));
            }
            await PopulateDropDowns();
            return View(response.Result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubscriptionVM model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDowns();
                return View(model);
            }

            var response = await _subscriptionService.UpdateAsync(model);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                await PopulateDropDowns();
                return View(model);
            }

            TempData["Success"] = "Subscription updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _subscriptionService.DeleteAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
            }
            else
            {
                TempData["Success"] = "Subscription deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var response = await _subscriptionService.CancelSubscriptionAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                return Json(new { success = false, message = response.ErrorMessage });
            }
            return Json(new { success = true, message = "Subscription cancelled successfully!" });
        }

        [HttpPost]
        public async Task<IActionResult> Renew(int id)
        {
            var response = await _subscriptionService.RenewSubscriptionAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                return Json(new { success = false, message = response.ErrorMessage });
            }
            return Json(new { success = true, message = "Subscription renewed successfully!" });
        }

        private async Task PopulateDropDowns()
        {
            var members = await _memberService.GetAllMembersAsync();
            var memberships = await _membershipService.GetActiveAsync();

            ViewBag.Members = new SelectList(members.Result ?? new List<MemberVM>(), "Id", "FullName");
            ViewBag.Memberships = new SelectList(memberships.Result ?? new List<MembershipVM>(), "Id", "MembershipType");
        }
    }
}
