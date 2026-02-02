using GymBLL.ModelVM.AI;
using System.Threading.Tasks;

namespace GymBLL.Service.Abstract.AI
{
    public interface IAIService
    {
        Task<AIChatResponse> ProcessMessageAsync(string userId, string message, bool isTrainer = false);
        Task<PlanSuggestionVM> SuggestDietPlanAsync(string memberId);
        Task<PlanSuggestionVM> SuggestWorkoutPlanAsync(string memberId);
        Task<GeneratedWorkoutPlanVM> GenerateWorkoutPlanAsync(AIWorkoutRequest request);
        Task<int> SaveGeneratedWorkoutPlanAsync(GeneratedWorkoutPlanVM generatedPlan, string creatorId);
    }
}
