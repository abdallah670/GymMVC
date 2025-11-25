using GymDAL.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymDAL.Entities.Workout
{
    
    public class WorkoutPlan : BaseEntity
    {
        [Required]
        public string TrainerId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Range(1, 365)]
        public int DurationDays { get; set; }
        [StringLength(50)]
        public string Difficulty { get; set; } = "Beginner"; // "Beginner", "Intermediate", "Advanced"

        [StringLength(50)]
        public string Goal { get; set; } = "General Fitness"; // "Weight Loss", "Muscle Gain", "Endurance", "General Fitness"

        public bool IsPublic { get; set; } = false; // Can be assigned to multiple members

        // Navigation properties
        public virtual Trainer Trainer { get; set; }
        public virtual ICollection<WorkoutPlanItem> WorkoutPlanItems { get; set; }
        public virtual ICollection<WorkoutAssignment> WorkoutAssignments { get; set; }
        public virtual ICollection<Membership> Memberships { get; set; } = new List<Membership>();
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
