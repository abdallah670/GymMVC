using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymDAL.Entities.Nutrition
{
    public class MealLog : BaseEntity
    {
        [Required]
        public int DietPlanAssignmentId { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [Required]
        public string MealsConsumed { get; set; } // JSON string or structured data

        [StringLength(1000)]
        public string? Notes { get; set; }

        [Range(0, 5000)]
        public int? CaloriesConsumed { get; set; }

        // Navigation properties
        public virtual DietPlanAssignment DietPlanAssignment { get; set; }
        public Member? Member { get; internal set; }
        [Required]
        public string MemberId { get; set; }
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
