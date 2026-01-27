using GymBLL.ModelVM.AI;
using GymBLL.Service.Abstract.AI;
using GymPL.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymPL.Controllers
{
    [Authorize]
    public class AIController : Controller
    {
        private readonly IAIService _aiService;

        public AIController(IAIService aiService)
        {
            _aiService = aiService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] AIChatRequest request)
        {
            if (string.IsNullOrEmpty(request.Message))
                return BadRequest("Message cannot be empty");

            var userId = User.GetUserId();
            var response = await _aiService.ProcessMessageAsync(userId, request.Message);

            return Json(response);
        }
    }
}
