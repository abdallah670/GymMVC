using GymDAL.Entities.Users;
using GymDAL.Entities.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymDAL.Entities.Workout
{
    public class WorkoutAssignment : BaseEntity
    {
        public int Id { get; set; }
        [Required]
        public int WorkoutPlanId { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        // Navigation properties
        public virtual WorkoutPlan WorkoutPlan { get; set; }
       
        public bool ToggleStatus()
        {
           
                this.IsActive = !this.IsActive;
             
                return true;
           
        }
    }

}
