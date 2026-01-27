using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GymDAL.Entities.Core;

namespace GymDAL.Entities.Nutrition
{
    public class DietPlanItem : BaseEntity
    {
        public int Id { get; set; }
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
        public string? ProteinMacros { get; set; }
        [StringLength(100)]
        public string? CarbMacros { get; set; }
        [StringLength(100)]
        public string? FatMacros { get; set; }
        [StringLength(500)]
        public string? Ingredients { get; set; }
        [StringLength(500)]
        public string? Notes { get; set; }
        // Navigation properties
        public virtual DietPlan DietPlan { get; set; }
        public bool ToggleStatus()
        {
            this.IsActive = !this.IsActive;
            return true;
        }
    }
}
