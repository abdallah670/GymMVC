using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymDAL.Entities.Nutrition
{
    public class MealLog : BaseEntity
    {
        public int Id { get; set; }
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
      
        public bool ToggleStatus()
        {
          
            this.IsActive = !this.IsActive;
               
            return true;
        }
    }
}
