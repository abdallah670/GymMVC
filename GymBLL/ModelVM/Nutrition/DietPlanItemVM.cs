using System.ComponentModel.DataAnnotations;

namespace GymBLL.ModelVM.Nutrition
{
    public class DietPlanItemVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Diet plan is required")]
        public int DietPlanId { get; set; }

        public string? DietPlanName { get; set; }

        [Required(ErrorMessage = "Day number is required")]
        [Range(1, 365, ErrorMessage = "Day must be between 1 and 365")]
        public int DayNumber { get; set; }

        [Required(ErrorMessage = "Meal type is required")]
        [StringLength(50)]
        public string MealType { get; set; } // Breakfast, Lunch, Dinner, Snack

        [Required(ErrorMessage = "Meal name is required")]
        [StringLength(100)]
        public string MealName { get; set; }

        [Range(0, 5000, ErrorMessage = "Calories must be between 0 and 5000")]
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

        public bool IsActive { get; set; } = true;
    }
}
