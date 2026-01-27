using GymBLL.ModelVM.Financial;
using GymBLL.Service.Abstract.Financial;
using GymBLL.Service.Implementation.Financial;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymPL.Controllers
{
    [Authorize(Roles = "Admin,Trainer")]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _paymentService.GetAllAsync();
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return View(new List<PaymentVM>());
            }
            return View(response.Result);
        }

        public async Task<IActionResult> Details(int id)
        {
            var response = await _paymentService.GetByIdAsync(id);
            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return RedirectToAction(nameof(Index));
            }
            return View(response.Result);
        }
    }
}
