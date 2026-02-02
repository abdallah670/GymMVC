using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using System.Text.Json.Serialization;
using GymBLL.Service.Abstract.AI;
using GymBLL.ModelVM.AI;
using GymDAL.Repo.Abstract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymDAL.Entities.Workout;

namespace GymBLL.Service.Implementation.AI
{
    public class AIService : IAIService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public AIService(IUnitOfWork unitOfWork, IConfiguration configuration, HttpClient httpClient)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<AIChatResponse> ProcessMessageAsync(string userId, string message, bool isTrainer = false)
        {
            try
            {
                var apiKey = _configuration["GeminiSettings:ApiKey"];
                if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_GEMINI_API_KEY_HERE")
                {
                    return new AIChatResponse { Reply = "AI Chat is not configured. Please add your Gemini API Key to appsettings.json.", IsError = true };
                }

                // 1. Gather Context
                var contextBuilder = new StringBuilder();
                if (isTrainer)
                {
                    contextBuilder.AppendLine("You are 'MenoPro Trainer AI', an expert fitness consultant and administrative assistant for gym trainers.");
                    contextBuilder.AppendLine("Your goal is to help trainers manage their members, analyze progress, and suggest optimization strategies.");
                    contextBuilder.AppendLine("Your tone is professional, technical, and efficient.");
                }
                else
                {
                    contextBuilder.AppendLine("You are 'MenoPro AI', an elite, highly professional, and motivating fitness coach at MenoPro Gym.");
                    contextBuilder.AppendLine("Your voice is encouraging, authoritative yet friendly, and science-based.");
                }
                
                contextBuilder.AppendLine("\nCORE RULES:");
                contextBuilder.AppendLine("1. If an exercise is mentioned, emphasize proper form and safety.");
                contextBuilder.AppendLine("2. If the user asks for nutritional advice, reference their assigned calories but warn that you are an AI assistant.");
                contextBuilder.AppendLine("3. Use the user's specific workout/diet info below to personalize every answer.");
                contextBuilder.AppendLine("4. Keep responses sharp, punchy, and under 150 words.");
                contextBuilder.AppendLine("5. Never mention you are a 'large language model'. You are MenoPro AI.");

                contextBuilder.AppendLine("\nCONTEXT DATA:");
                contextBuilder.AppendLine(await GetWorkoutContext(userId));
                contextBuilder.AppendLine(await GetDietContext(userId));
                contextBuilder.AppendLine(await GetHistoryContext(userId));

                contextBuilder.AppendLine("\nAnswer the user's question based on this context if relevant, else answer generally as a fitness expert.");
                contextBuilder.AppendLine("Keep responses concise and motivating. Use emojis occasionally.");

                // 2. Prepare Gemini Request
                var prompt = $"{contextBuilder}\n\nUser Question: {message}";
                var requestBody = new
                {
                    contents = new[]
                    {
                        new { parts = new[] { new { text = prompt } } }
                    }
                };

                // 3. Call Gemini API
                var response = await _httpClient.PostAsJsonAsync($"https://generativelanguage.googleapis.com/v1beta/models/gemini-flash-latest:generateContent?key={apiKey}", requestBody);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new AIChatResponse { Reply = $"API Error: {response.StatusCode}", IsError = true };
                }

                var geminiResult = await response.Content.ReadFromJsonAsync<AIResponse>();
                var reply = geminiResult?.Candidates?[0]?.Content?.Parts?[0]?.Text ?? "I'm not sure how to answer that.";

                return new AIChatResponse 
                { 
                    Reply = reply.Trim(),
                    SuggestedActions = GenerateSuggestedActions(reply)
                };
            }
            catch (Exception ex)
            {
                return new AIChatResponse { Reply = "An error occurred while processing your request.", IsError = true };
            }
        }

        public async Task<PlanSuggestionVM> SuggestDietPlanAsync(string memberId)
        {
            try
            {
                var member = await _unitOfWork.Members.Get(m => m.Id == memberId, include: q => q.Include(m => m.FitnessGoal)).FirstOrDefaultAsync();
                if (member == null) return new PlanSuggestionVM { IsError = true, ErrorMessage = "Member not found" };

                var prompt = $@"
You are a professional nutritionist. Generate a 1-day diet plan for the following gym member as a JSON object.
MEMBER DATA:
- Name: {member.FullName}
- Age: {CalculateAge(member.JoinDate)} (Approx)
- Weight: {member.CurrentWeight} kg
- Height: {member.Height} cm
- Gender: {member.Gender}
- Goal: {member.FitnessGoal?.GoalName}
- Activity Level: {member.ActivityLevel}

REQUIRED JSON FORMAT:
{{
  ""PlanName"": ""String (e.g., Lean Bulk High Protein)"",
  ""Description"": ""Short overview"",
  ""Goal"": ""The target goal"",
  ""TotalCalories"": 2500,
  ""RecommendedDietType"": ""Bulk/Cut/Maintenance"",
  ""SuggestedMeals"": [
    {{ ""MealName"": ""Oatmeal"", ""MealType"": ""Breakfast"", ""Notes"": ""Oatmeal with berries and nuts"", ""Calories"": 400 }},
    {{ ""MealName"": ""Grilled Chicken"", ""MealType"": ""Lunch"", ""Notes"": ""Chicken with brown rice and broccoli"", ""Calories"": 600 }},
    {{ ""MealName"": ""Pan-Seared Salmon"", ""MealType"": ""Dinner"", ""Notes"": ""Salmon with asparagus and quinoa"", ""Calories"": 550 }}
  ],
  ""Reasoning"": ""Why this plan fits the member""
}}
Return ONLY the RAW JSON object. No markdown, no backticks.";

                return await CallGeminiForStructuredData<PlanSuggestionVM>(prompt);
            }
            catch (Exception ex)
            {
                return new PlanSuggestionVM { IsError = true, ErrorMessage = ex.Message };
            }
        }

        public async Task<PlanSuggestionVM> SuggestWorkoutPlanAsync(string memberId)
        {
            try
            {
                var member = await _unitOfWork.Members.Get(m => m.Id == memberId, include: q => q.Include(m => m.FitnessGoal)).FirstOrDefaultAsync();
                if (member == null) return new PlanSuggestionVM { IsError = true, ErrorMessage = "Member not found" };

                var prompt = $@"
You are a professional fitness coach. Generate a targeted workout session for the following gym member as a JSON object.
MEMBER DATA:
- Name: {member.FullName}
- Weight: {member.CurrentWeight} kg
- Goal: {member.FitnessGoal?.GoalName}
- Activity Level: {member.ActivityLevel}

REQUIRED JSON FORMAT:
{{
  ""PlanName"": ""String (e.g., Full Body Hypertrophy)"",
  ""Description"": ""Short overview"",
  ""DifficultyLevel"": ""Beginner/Intermediate/Advanced"",
  ""SuggestedExercises"": [
    {{ ""ExerciseName"": ""Bench Press"", ""Sets"": ""3"", ""Reps"": ""12"", ""Notes"": ""Keep elbows tucked"" }},
    {{ ""ExerciseName"": ""Squats"", ""Sets"": ""4"", ""Reps"": ""10"", ""Notes"": ""Go below parallel"" }}
  ],
  ""Reasoning"": ""Why this plan fits the member""
}}
Return ONLY the RAW JSON object. No markdown, no backticks.";

                return await CallGeminiForStructuredData<PlanSuggestionVM>(prompt);
            }
            catch (Exception ex)
            {
                return new PlanSuggestionVM { IsError = true, ErrorMessage = ex.Message };
            }
        }

        public async Task<GeneratedWorkoutPlanVM> GenerateWorkoutPlanAsync(AIWorkoutRequest request)
        {
            try
            {
                var prompt = $@"
You are an expert fitness coach. Create a comprehensive 4-week workout plan based on the following requirements:
- Goal: {request.Goal}
- Experience Level: {request.ExperienceLevel}
- Days Per Week: {request.DaysPerWeek}
- Equipment: {request.Equipment}
- Duration Per Session: {request.DurationPerSession}
- Notes: {request.AdditionalNotes}

REQUIRED JSON FORMAT:
{{
  ""Name"": ""Exciting Plan Name"",
  ""Description"": ""Detailed description of the plan philosophy"",
  ""Level"": ""{request.ExperienceLevel}"",
  ""DurationWeeks"": 4,
  ""Schedule"": [
    {{
      ""DayNumber"": 1,
      ""DayName"": ""e.g. Chest & Triceps"",
      ""Exercises"": [
        {{ ""Name"": ""Bench Press"", ""Sets"": ""3"", ""Reps"": ""10"", ""Note"": ""Focus on control"" }}
      ]
    }}
  ]
}}
Ensure the schedule covers exactly {request.DaysPerWeek} training days for a typical week.
Return ONLY the RAW JSON object. No markdown.";

                return await CallGeminiForStructuredData<GeneratedWorkoutPlanVM>(prompt);
            }
            catch (Exception ex)
            {
                return new GeneratedWorkoutPlanVM { Name = "Error", Description = "Failed to generate plan: " + ex.Message };
            }
        }

        public async Task<int> SaveGeneratedWorkoutPlanAsync(GeneratedWorkoutPlanVM generatedPlan, string creatorId)
        {
            try
            {
                var newPlan = new GymBLL.ModelVM.Workout.WorkoutPlanVM
                {
                    Name = generatedPlan.Name ?? "AI Generated Plan",
                    Description = generatedPlan.Description ?? "Generated by Gemini AI",
                    Difficulty = generatedPlan.Level ?? "Beginner",
                    DurationWeeks = generatedPlan.DurationWeeks,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    Goal = "General Fitness" // Default, could be mapped if Goal is in VM
                };

                // Create Plan first (we'll need to map to Entity or use Service directly if possible, assuming BLL here)
                // Since this is AIService, we should use UnitOfWork or specialized Service. 
                // Let's use UnitOfWork to map to Entity manually if AutoMapper isn't injected, 
                // BUT typically we should use WorkoutPlanService. Let's assume we can map manualy to Entity for now to be safe.
                
                var planEntity = new WorkoutPlan
                {
                    Name = newPlan.Name,
                    Description = newPlan.Description,
                    Difficulty = newPlan.Difficulty,
                    DurationWeeks = newPlan.DurationWeeks,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    Goal = newPlan.Goal, // Ensure Goal is present in Entity
                   
                };

                await _unitOfWork.WorkoutPlans.AddAsync(planEntity);
                await _unitOfWork.SaveAsync(); // Save to get ID

                if (generatedPlan.Schedule != null)
                {
                    var items = new List<WorkoutPlanItem>();
                    foreach (var day in generatedPlan.Schedule)
                    {
                        foreach (var ex in day.Exercises)
                        {
                            items.Add(new WorkoutPlanItem
                            {
                                WorkoutPlanId = planEntity.Id,
                                DayNumber = day.DayNumber,
                                ExerciseName = ex.Name,
                                Sets = ex.Sets.ToString(),
                                Reps = ex.Reps.ToString(),
                                Notes = ex.Note,
                                IsActive = true
                            });
                        }
                    }
                    await _unitOfWork.WorkoutPlanItems.AddRangeAsync(items);
                    await _unitOfWork.SaveAsync();
                }

                return planEntity.Id;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save plan: {ex.Message}");
            }
        }

        private async Task<T> CallGeminiForStructuredData<T>(string prompt)
        {
            var apiKey = _configuration["GeminiSettings:ApiKey"];
            var requestBody = new
            {
                contents = new[] { new { parts = new[] { new { text = prompt } } } }
            };

            var response = await _httpClient.PostAsJsonAsync($"https://generativelanguage.googleapis.com/v1beta/models/gemini-flash-latest:generateContent?key={apiKey}", requestBody);
            
            if (!response.IsSuccessStatusCode) throw new Exception("AI API Error");

            var result = await response.Content.ReadFromJsonAsync<AIResponse>();
            var rawJson = result?.Candidates?[0]?.Content?.Parts?[0]?.Text ?? "";
            
            // Clean markdown if present
            rawJson = rawJson.Replace("```json", "").Replace("```", "").Trim();

            return JsonSerializer.Deserialize<T>(rawJson, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            });
        }

        private async Task<string> GetWorkoutContext(string userId)
        {
            var subscritpion = await _unitOfWork.Subscriptions.GetActiveSubscriptionByMemberIdAsync(userId);
            if (subscritpion != null && subscritpion.WorkoutAssignmentId != null)
            {
                var assignment = await _unitOfWork.WorkoutAssignments.GetDetailed(subscritpion.WorkoutAssignmentId.Value);
                if (assignment != null)
                {
                    var dayNumber = (int)(DateTime.UtcNow.Date - assignment.StartDate.Date).TotalDays + 1;
                    return $"- Workout Plan: {assignment.WorkoutPlan?.Name}. Current Day: {dayNumber}.";
                }
            }
            return "- No active workout plan assigned.";
        }

        private async Task<string> GetDietContext(string userId)
        {
             var subscritpion = await _unitOfWork.Subscriptions.GetActiveSubscriptionByMemberIdAsync(userId);
             if (subscritpion != null && subscritpion.DietPlanAssignmentId != null)
             {
                 var assignment = await _unitOfWork.DietPlanAssignments.GetDetailed(subscritpion.DietPlanAssignmentId.Value);
                 if (assignment != null)
                 {
                     return $"- Diet Plan: {assignment.DietPlan?.Name} ({assignment.DietPlan?.TotalCalories} kcal).";
                 }
             }
             return "- No active diet plan assigned.";
        }

        private async Task<string> GetHistoryContext(string userId)
        {
            var lastLog = await _unitOfWork.WorkoutLogs
                .Get(l => l.MemberId == userId)
                .OrderByDescending(l => l.Date)
                .FirstOrDefaultAsync();

            if (lastLog != null)
            {
                var daysAgo = (int)(DateTime.UtcNow - lastLog.Date).TotalDays;
                return $"- Last Workout: {lastLog.DurationMinutes} mins, {daysAgo} days ago.";
            }
            return "- No workout logs yet.";
        }

        private List<string> GenerateSuggestedActions(string reply)
        {
            var actions = new List<string>();
            var lowerReply = reply.ToLower();
            if (lowerReply.Contains("workout") || lowerReply.Contains("exercise")) actions.Add("View Workout");
            if (lowerReply.Contains("diet") || lowerReply.Contains("meal") || lowerReply.Contains("eat")) actions.Add("View Diet");
            if (lowerReply.Contains("log") || lowerReply.Contains("progress")) actions.Add("My Progress");
            
            if (actions.Count == 0) actions.Add("Check Workout");

            return actions.Distinct().Take(3).ToList();
        }

        private int CalculateAge(DateTime joinDate)
        {
            // Simplified age calc if DOB is missing, though we should use DOB if avail
            var now = DateTime.UtcNow;
            int age = now.Year - joinDate.Year;
            if (now < joinDate.AddYears(age)) age--;
            return age;


        }

        // Gemini Response Models
        private class AIResponse
        {
            [JsonPropertyName("candidates")]
            public List<Candidate> Candidates { get; set; }
        }

        private class Candidate
        {
            [JsonPropertyName("content")]
            public Content Content { get; set; }
        }

        private class Content
        {
            [JsonPropertyName("parts")]
            public List<Part> Parts { get; set; }
        }

        private class Part
        {
            [JsonPropertyName("text")]
            public string Text { get; set; }
        }
        }
}
