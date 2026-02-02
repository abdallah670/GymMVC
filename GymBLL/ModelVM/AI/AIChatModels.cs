using System.Collections.Generic;

namespace GymBLL.ModelVM.AI
{
    public class AIChatRequest
    {
        public string Message { get; set; }
        // We can add history here later for context awareness
    }

    public class AIChatResponse
    {
        public string Reply { get; set; }
        public List<string> SuggestedActions { get; set; } = new List<string>();
        public bool IsError { get; set; }
    }

    public class AIWorkoutRequest
    {
        public string Goal { get; set; }
        public string ExperienceLevel { get; set; }
        public int DaysPerWeek { get; set; }
        public string Equipment { get; set; }
        public string DurationPerSession { get; set; }
        public string AdditionalNotes { get; set; }
    }

    public class GeneratedWorkoutPlanVM
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Level { get; set; }
        public int DurationWeeks { get; set; } = 4;
        public List<GeneratedWorkoutDay> Schedule { get; set; } = new List<GeneratedWorkoutDay>();
    }

    public class GeneratedWorkoutDay
    {
        public int DayNumber { get; set; }
        public string DayName { get; set; } // e.g. "Upper Body"
        public List<GeneratedExercise> Exercises { get; set; } = new List<GeneratedExercise>();
    }

    public class GeneratedExercise
    {
        public string Name { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; } // Keep simple int for now, or string if range
        public string Note { get; set; }
    }
}
