using System;
using System.ComponentModel.DataAnnotations;

namespace GymBLL.ModelVM.Workout
{
    public class WorkoutPlanVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [StringLength(50)]
        public string Difficulty { get; set; } = "Beginner"; // "Beginner", "Intermediate", "Advanced"

        [StringLength(50)]
        public string Goal { get; set; } = "General Fitness";
        // "Weight Loss", "Muscle Gain", "Endurance", "General Fitness"
        public int DurationWeeks { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<WorkoutPlanItemVM>? workoutPlanItemVMs { get; set; }
    }
}
