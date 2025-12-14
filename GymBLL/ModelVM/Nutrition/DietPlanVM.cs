using System;
using System.ComponentModel.DataAnnotations;

namespace GymBLL.ModelVM.Nutrition
{
    public class DietPlanVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [Range(0, 10000, ErrorMessage = "Total calories must be between 0 and 10000")]
        public int? TotalCalories { get; set; }

        [StringLength(100)]
        public string? ProteinMacros { get; set; }

        [StringLength(100)]
        public string? CarbMacros { get; set; }

        [StringLength(100)]
        public string? FatMacros { get; set; } 
        public string DietType { get; set; } = "Balanced"; // "Keto", "Vegetarian", "Vegan", "High-Protein", "Balanced"

        public int DurationDays { get; set; } = 30; // Default 30-day plan

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<DietPlanItemVM>? DietPlanItemsVM { get; set; }
    }
}
