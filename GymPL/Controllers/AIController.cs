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

        [HttpGet]
        public IActionResult WorkoutGenerator()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GenerateWorkout([FromBody] AIWorkoutRequest request)
        {
            if (request == null) return BadRequest("Invalid request");

            var plan = await _aiService.GenerateWorkoutPlanAsync(request);
            
            return Json(plan);
        }

        [HttpPost]
        public async Task<IActionResult> SavePlan([FromBody] GeneratedWorkoutPlanVM plan)
        {
            if (plan == null) return BadRequest("Invalid plan data");

            try
            {
                var userId = User.GetUserId();
                var planId = await _aiService.SaveGeneratedWorkoutPlanAsync(plan, userId);
                return Json(new { success = true, planId = planId });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] AIChatRequest request)
        {
            if (string.IsNullOrEmpty(request.Message))
                return BadRequest("Message cannot be empty");

            var userId = User.GetUserId();
            var isTrainer = User.IsInRole("Trainer");
            var response = await _aiService.ProcessMessageAsync(userId, request.Message, isTrainer);

            return Json(response);
        }
    }
}
