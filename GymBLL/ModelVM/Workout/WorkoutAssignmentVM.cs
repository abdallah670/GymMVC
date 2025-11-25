using System;
using System.ComponentModel.DataAnnotations;

namespace GymBLL.ModelVM.Workout
{
    public class WorkoutAssignmentVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Workout plan is required")]
        public int WorkoutPlanId { get; set; }
        public WorkoutPlanVM? WorkoutPlan { get; set; }
        [Required(ErrorMessage = "Start date is required")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
