using GymDAL.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymDAL.Entities.Nutrition
{
    public class DietPlanAssignment : BaseEntity
    {
        [Required]
        public int DietPlanId { get; set; }

        [Required]
        public string MemberId { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        // Navigation properties
        public virtual DietPlan DietPlan { get; set; }
        public virtual Member Member { get; set; }
        public virtual ICollection<MealLog> MealLogs { get; set; }
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
