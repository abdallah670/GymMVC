using GymDAL.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GymDAL.Entities.Core;

namespace GymDAL.Entities.Workout
{
    
    public class WorkoutPlan : BaseEntity
    {
       public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(1000)]
        public string? Description { get; set; }
        public int DurationWeeks { get; set; } = 4; // Default 4-week plan
        [StringLength(50)]
        public string Difficulty { get; set; } = "Beginner"; // "Beginner", "Intermediate", "Advanced"
        [StringLength(50)]
        public string Goal { get; set; } = "General Fitness"; // "Weight Loss", "Muscle Gain", "Endurance", "General Fitness"
        public virtual ICollection<WorkoutPlanItem> WorkoutPlanItems { get; set; }
        public virtual ICollection<WorkoutAssignment> WorkoutAssignments { get; set; }
       
       
    }
}
