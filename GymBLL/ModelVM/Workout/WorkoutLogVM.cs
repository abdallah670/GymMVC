using System;
using System.Collections.Generic;

namespace GymBLL.ModelVM.Workout
{
    public class WorkoutLogVM
    {
        public int Id { get; set; }
        public string MemberId { get; set; }
        public int? WorkoutPlanId { get; set; }
        public string WorkoutPlanName { get; set; }
        
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public int DurationMinutes { get; set; }
        public string? Notes { get; set; }

        public List<WorkoutLogEntryVM> Entries { get; set; } = new List<WorkoutLogEntryVM>();
    }

    public class WorkoutLogEntryVM
    {
        public int Id { get; set; }
        public int? WorkoutPlanItemId { get; set; } // Link back to original plan item if applicable
        public string ExerciseName { get; set; }
        
        public int SetsPerformed { get; set; }
        public string? RepsPerformed { get; set; } // e.g., "12, 10, 8"
        public string? WeightLifted { get; set; } // e.g., "60, 60, 60"
        
        public string? TargetSets { get; set; }
        public string? TargetReps { get; set; }
    }
}
