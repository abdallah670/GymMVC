using System.ComponentModel.DataAnnotations;

namespace GymBLL.ModelVM.Workout
{
    public class WorkoutPlanItemVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Workout plan is required")]
        public int WorkoutPlanId { get; set; }



        [Required(ErrorMessage = "Day number is required")]
        [Range(1, 7, ErrorMessage = "Day must be between 1 and 7")]
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

        [Url(ErrorMessage = "Please enter a valid URL (e.g., https://youtube.com/watch?v=example)")]
        [StringLength(500)]
        [Display(Name = "Video URL")]
        public string? VideoUrl { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
