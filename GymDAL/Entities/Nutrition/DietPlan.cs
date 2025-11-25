using GymDAL.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymDAL.Entities.Nutrition
{
    public class DietPlan : BaseEntity
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]

        [StringLength(1000)]
        public string? Description { get; set; }
        [Range(0, 10000)]
        public int? TotalCalories { get; set; }
        [StringLength(100)]
        public string? ProteinMacros { get; set; } // "40% Protein, 30% Carbs, 30% Fat"
        [StringLength(100)]
        public string? CarbMacros { get; set; } // "40% Protein, 30% Carbs, 30% Fat"
        [StringLength(100)]
        public string? FatMacros { get; set; } // "40% Protein, 30% Carbs, 30% Fat"
        [StringLength(50)]
        public string DietType { get; set; } = "Balanced"; // "Keto", "Vegetarian", "Vegan", "High-Protein", "Balanced"
        public int DurationDays { get; set; } = 30; // Default 30-day plan
        public virtual ICollection<DietPlanItem> DietPlanItems { get; set; } = new List<DietPlanItem>();
        public virtual ICollection<Membership> Memberships { get; set; } = new List<Membership>();
        public virtual ICollection<DietPlanAssignment> DietPlanAssignments { get; set; } = new List<DietPlanAssignment>();
        public bool ToggleStatus()
        {
           
                this.IsActive = !this.IsActive;
               
                return true;
            
        }
    }

   

   

 
}
