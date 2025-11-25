using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymDAL.Entities.Nutrition
{
    public class DietPlanItem : BaseEntity
    {
        [Required]
        public int DietPlanId { get; set; }

        [Range(1, 365)]
        public int DayNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string MealType { get; set; } // Breakfast, Lunch, Dinner, Snack

        [Required]
        [StringLength(100)]
        public string MealName { get; set; }

        [Range(0, 5000)]
        public int Calories { get; set; }

        [StringLength(100)]
        public string? Macros { get; set; }

        [StringLength(500)]
        public string? Ingredients { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        // Navigation properties
        public virtual DietPlan DietPlan { get; set; }
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
