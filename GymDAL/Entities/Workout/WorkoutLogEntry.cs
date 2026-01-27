using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymDAL.Entities.Workout
{
    public class WorkoutLogEntry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int WorkoutLogId { get; set; }
        [ForeignKey("WorkoutLogId")]
        public virtual WorkoutLog WorkoutLog { get; set; }

        public int? WorkoutPlanItemId { get; set; }
        [ForeignKey("WorkoutPlanItemId")]
        public virtual WorkoutPlanItem WorkoutPlanItem { get; set; }

        [Required]
        [StringLength(100)]
        public string ExerciseName { get; set; }

        public int SetsPerformed { get; set; }

        [StringLength(50)]
        public string? RepsPerformed { get; set; } // e.g. "12, 12, 10"

        [StringLength(50)]
        public string? WeightLifted { get; set; } // e.g. "50, 50, 55"
    }
}
