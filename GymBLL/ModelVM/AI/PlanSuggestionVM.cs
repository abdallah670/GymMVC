using GymBLL.ModelVM.Nutrition;
using GymBLL.ModelVM.Workout;
using System.Collections.Generic;

namespace GymBLL.ModelVM.AI
{
    public class PlanSuggestionVM
    {
        public string PlanName { get; set; }
        public string Description { get; set; }
        public string Type { get; set; } // "Diet" or "Workout"
        
        // For Diet
        public double? TotalCalories { get; set; }
        public string RecommendedDietType { get; set; }
        public List<DietPlanItemVM> SuggestedMeals { get; set; } = new List<DietPlanItemVM>();

        // For Workout
        public int? DurationWeeks { get; set; }
        public string DifficultyLevel { get; set; }
        public List<WorkoutPlanItemVM> SuggestedExercises { get; set; } = new List<WorkoutPlanItemVM>();

        public string Reasoning { get; set; }
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
    }
}
