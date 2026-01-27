using GymBLL.ModelVM.AI;
using GymBLL.Service.Abstract.AI;
using GymDAL.Repo.Abstract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymBLL.Service.Implementation.AI
{
    public class AIService : IAIService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AIService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AIChatResponse> ProcessMessageAsync(string userId, string message)
        {
            try
            {
                var input = message.ToLower().Trim();

                // 1. Identify Intent
                if (input.Contains("workout") || input.Contains("exercise") || input.Contains("train") || input.Contains("today"))
                {
                    return await GetWorkoutResponse(userId);
                }
                else if (input.Contains("diet") || input.Contains("food") || input.Contains("eat") || input.Contains("meal"))
                {
                    return await GetDietResponse(userId);
                }
                else if (input.Contains("history") || input.Contains("log") || input.Contains("progress") || input.Contains("last"))
                {
                    return await GetHistoryResponse(userId);
                }
                else if (input.Contains("trainer") || input.Contains("coach"))
                {
                    return await GetTrainerResponse(userId);
                }
                else if (input.Contains("hello") || input.Contains("hi"))
                {
                     return new AIChatResponse { Reply = "Hello! I'm your Gym Bot. Ask me about your 'workout', 'diet', or 'progress'!" };
                }

                return new AIChatResponse 
                { 
                    Reply = "I'm not sure about that. Try asking: 'What is my workout today?' or 'Show my diet plan'.",
                    SuggestedActions = new List<string> { "Check Workout", "View Diet", "My Progress" }
                };
            }
            catch (Exception ex)
            {
                return new AIChatResponse { Reply = "My circuits are fried (Error processing request).", IsError = true };
            }
        }

        private async Task<AIChatResponse> GetWorkoutResponse(string userId)
        {
            
            var subscritpion=await _unitOfWork.Subscriptions.GetByIdAsync(userId);
            if (subscritpion != null)
            {
                var assignment = await _unitOfWork.WorkoutAssignments
                    .GetDetailed(subscritpion.WorkoutAssignmentId);
                if (assignment == null)
                    return new AIChatResponse { Reply = "You don't have an active workout plan assigned. Ask your trainer to set one up!" };

                var dayNumber = (int)(DateTime.UtcNow.Date - assignment.StartDate.Date).TotalDays + 1;

                // Basic logic: wrap day number if it exceeds duration (cyclic) or just show current day
                // For simplicity, let's just say what the plan is.
                return new AIChatResponse
                {
                    Reply = $"You are on Day {dayNumber} of the '{assignment.WorkoutPlan.Name}' plan. It's time to crush it! 💪",
                    SuggestedActions = new List<string> { "View Workout", "Log Session" }
                };
            }
            else
            {
                return new AIChatResponse
                {
                    Reply = "You don't have an active Subscription plan assigned. renew your subscription!"
                };
              }
        }

        private async Task<AIChatResponse> GetDietResponse(string userId)
        {
            var subscritpion = await _unitOfWork.Subscriptions.GetByIdAsync(userId);
            if (subscritpion != null)
            {
                var assignment = await _unitOfWork.DietPlanAssignments.GetDetailed(subscritpion.DietPlanAssignmentId);
                 if (assignment == null)
                    return new AIChatResponse { Reply = "No diet plan found. Remember, abs are made in the kitchen! Contact your trainer." };

                return new AIChatResponse
                {
                    Reply = $"You are following '{assignment.DietPlan.Name}'. Target: {assignment.DietPlan.TotalCalories} calories. Eat clean! 🥦",
                    SuggestedActions = new List<string> { "View Diet" }
                };
            }
            else
            {
                return new AIChatResponse
                {
                    Reply = "You don't have an active Subscription plan assigned. renew your subscription!"
                };
            }
          

          
        }

        private async Task<AIChatResponse> GetHistoryResponse(string userId)
        {
            var lastLog = await _unitOfWork.WorkoutLogs
                .Get(l => l.MemberId == userId)
                .OrderByDescending(l => l.Date)
                .FirstOrDefaultAsync();

            if (lastLog == null)
                return new AIChatResponse { Reply = "I don't see any logs yet. Go do a workout and log it!" };

            var daysAgo = (int)(DateTime.UtcNow - lastLog.Date).TotalDays;
            var when = daysAgo == 0 ? "today" : $"{daysAgo} days ago";

            return new AIChatResponse 
            { 
                Reply = $"Your last workout was {when}. You did '{lastLog.DurationMinutes} mins'. Keep the momentum going! 🔥",
                SuggestedActions = new List<string> { "View History" }
            };
        }
        
        private async Task<AIChatResponse> GetTrainerResponse(string userId)
        {
             return new AIChatResponse { Reply = "Your trainer wants you to succeed. You can message them directly in the Chat section." };
        }
    }
}
