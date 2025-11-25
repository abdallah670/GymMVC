using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymDAL.Entities.Workout
{
    public class WorkoutLog : BaseEntity
    {
        [Required]
        public int WorkoutAssignmentId { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [Required]
        public string CompletedExercises { get; set; } // JSON string or structured data

        [StringLength(1000)]
        public string? Notes { get; set; }

        public WorkoutStatus Status { get; set; } = WorkoutStatus.Pending;

        [Range(0, 500)]
        public int? DurationMinutes { get; set; }

        [Range(0, 5000)]
        public int? CaloriesBurned { get; set; }
        [Required]
        public string MemberId { get; set; }
        // Navigation properties
        public virtual WorkoutAssignment WorkoutAssignment { get; set; }
        public Member Member { get; internal set; }
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
