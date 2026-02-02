using GymBLL.ModelVM.Financial;
using GymBLL.Service.Abstract.Financial;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymPL.Controllers
{
    // Adjust roles according to your Gym roles. "Owner" and "Admin" seem appropriate.
    [Authorize(Roles = "Trainer,Admin")]
    public class MembershipController : Controller
    {
        private readonly IMembershipService _membershipService;

        public MembershipController(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _membershipService.GetAllAsync();
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return View(new List<MembershipVM>());
            }
            return View(response.Result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new MembershipVM
            {
                IsActive = true,
                HasWorkoutPlan = false,
                HasDietPlan = false
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MembershipVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _membershipService.CreateAsync(model);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return View(model);
            }

            TempData["Success"] = "Membership Type created successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _membershipService.GetByIdAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return RedirectToAction(nameof(Index));
            }
            return View(response.Result);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _membershipService.GetByIdAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return RedirectToAction(nameof(Index));
            }
            return View(response.Result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MembershipVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _membershipService.UpdateAsync(model);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return View(model);
            }

            TempData["Success"] = "Membership Type updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var response = await _membershipService.ToggleStatusAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
            }
            else
            {
                TempData["Success"] = "Membership status toggled successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _membershipService.DeleteAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = "Cannot delete membership as it is likely in use. Try disabling it instead.";
            }
            else
            {
                TempData["Success"] = "Membership Type deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
