using System;
using System.ComponentModel.DataAnnotations;

namespace GymBLL.ModelVM.Nutrition
{
    public class MealLogVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Diet plan assignment is required")]
        public int DietPlanAssignmentId { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "Meals consumed is required")]
        public string MealsConsumed { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        [Range(0, 5000, ErrorMessage = "Calories must be between 0 and 5000")]
        public int? CaloriesConsumed { get; set; }

        public bool IsActive { get; set; } = true;
        public DietPlanAssignmentVM? DietPlanAssignment { get; set; }
     
    }
}
