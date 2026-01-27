using GymDAL.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using GymDAL.Entities.Core;

namespace GymDAL.Entities.Nutrition
{
    public class DietPlanAssignment : BaseEntity
    {
        public int Id { get; set; }
        [Required]
        public int DietPlanId { get; set; }

     

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        // Navigation properties
        public virtual DietPlan? DietPlan { get; set; }
       
        public virtual ICollection<MealLog>? MealLogs { get; set; }
        public bool ToggleStatus()
        {
            this.IsActive = !this.IsActive;
            return true;
        }
    }
}
