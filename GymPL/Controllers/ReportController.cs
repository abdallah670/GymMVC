using GymBLL.Service.Abstract.Report;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymPL.Controllers
{
    [Authorize(Roles = "Trainer,Admin")]
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }
        [HttpGet]
        [Authorize(Roles = "Admin,Trainer,Nutritionist,Member")]
        public async Task<IActionResult> Index(string memberId, string returnUrl)
        {
            // If User is Member, ensure they can only view their own report
            if (User.IsInRole("Member"))
            {
                var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(currentUserId) || (memberId != null && memberId != currentUserId))
                {
                    return Forbid();
                }
                memberId = currentUserId; // Ensure memberId is set to current user
            }

            if (string.IsNullOrEmpty(memberId))
                return NotFound();

            var response = await _reportService.GenerateMemberReportAsync(memberId);

            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = "Could not generate report.";

                // Redirect back using returnUrl if provided
                if (!string.IsNullOrEmpty(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Dashboard", "Member");
            }

            // Pass returnUrl to view for the Back button
            ViewBag.ReturnUrl = returnUrl;

            return View(response.Result);
        }
    }
}
