using GymDAL.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymDAL.Entities.Workout
{
    public class WorkoutAssignment : BaseEntity,IGlobal
    {
        [Required]
        public int WorkoutPlanId { get; set; }

        [Required]
        public string MemberId { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        // Navigation properties
        public virtual WorkoutPlan WorkoutPlan { get; set; }
        public virtual Member Member { get; set; }
        public virtual ICollection<WorkoutLog> WorkoutLogs { get; set; }
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
