using System.ComponentModel.DataAnnotations;

namespace GymBLL.ModelVM.Workout
{
    public class WorkoutPlanItemVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Workout plan is required")]
        public int WorkoutPlanId { get; set; }

        public string? WorkoutPlanName { get; set; }

        [Required(ErrorMessage = "Day number is required")]
        [Range(1, 365, ErrorMessage = "Day must be between 1 and 365")]
        public int DayNumber { get; set; }

        [Required(ErrorMessage = "Exercise name is required")]
        [StringLength(100)]
        public string ExerciseName { get; set; }

        [StringLength(50)]
        public string? Sets { get; set; }

        [StringLength(50)]
        public string? Reps { get; set; }

        [StringLength(50)]
        public string? RestDuration { get; set; }

        [StringLength(100)]
        public string? Equipment { get; set; }

        [Url]
        [StringLength(500)]
        public string? VideoUrl { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
