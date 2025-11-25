using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymDAL.Entities.Workout
{
    public class WorkoutPlanItem : BaseEntity
    {
        [Required]
        public int WorkoutPlanId { get; set; }

        [Range(1, 365)]
        public int DayNumber { get; set; }

        [Required]
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

        // Navigation properties
        public virtual WorkoutPlan WorkoutPlan { get; set; }
        public bool ToggleStatus(string DeletedBy)
        {
            if (!string.IsNullOrEmpty(DeletedBy))
            {
                this.IsActive = !this.IsActive;
                this.DeletedBy = DeletedBy;
                this.DeletedAt = DateTime.UtcNow;
                return true;
            }
            return false;
        }
    }
}
