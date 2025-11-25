using System;
using System.ComponentModel.DataAnnotations;

namespace GymBLL.ModelVM.External
{
    public class MembershipVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Membership type is required")]
        [StringLength(50)]
        public string MembershipType { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Duration is required")]
        [Range(1, 120, ErrorMessage = "Duration must be between 1 and 120 months")]
        public int DurationInMonths { get; set; }

        public bool HasDietPlan { get; set; } = false;

        public bool HasWorkoutPlan { get; set; } = false;

        public bool? HasNotification { get; set; } = false;

        public bool? IsFollowedByTrainer { get; set; } = false;

        [DataType(DataType.Date)]
        public DateTime? CancellationDate { get; set; }

        [StringLength(500)]
        public string? CancellationReason { get; set; }

        [StringLength(50)]
        public string? PreferredTrainingTime { get; set; }

        [StringLength(20)]
        public string? TrainingIntensity { get; set; }

        

     
        [StringLength(250)]
        public string? Features { get; set; }
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
