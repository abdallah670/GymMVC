using System;
using System.ComponentModel.DataAnnotations;

namespace GymBLL.ModelVM.Nutrition
{
    public class DietPlanAssignmentVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Diet plan is required")]
        public int DietPlanId { get; set; }
        public DietPlanVM ? DietPlan { get; set; }

       

        public string? MemberName { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
