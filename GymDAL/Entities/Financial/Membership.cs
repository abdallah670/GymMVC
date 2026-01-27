using System.ComponentModel.DataAnnotations;
using GymDAL.Entities.Users;

namespace GymDAL.Entities.Financial
{
    public class Membership : BaseEntity
    {

        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string MembershipType { get; set; } // "Basic", "Premium", "VIP", "Student", "Corporate", "PersonalTraining"
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }
        public int DurationInMonths { get; set; } // Duration of membership in months
      
      
   
        public bool HasDietPlan { get; set; } = false;
        public bool HasWorkoutPlan { get; set; } = false;
        public bool? HasNotification { get; set; } = false;
        public bool? IsFollowedByTrainer { get; set; } = false;
        public DateTime? CancellationDate { get; set; }
        [StringLength(500)]
        public string? CancellationReason { get; set; }
        [StringLength(50)]
        public string? PreferredTrainingTime { get; set; } // "Morning", "Afternoon", "Evening"
        
        [StringLength(20)]
        public string? TrainingIntensity { get; set; } // "Light", "Moderate", "High"
                                           // Associated Plans
        [StringLength(250)]
        public string? Features { get; set; }

        public void ToggleStatus()
        {
            throw new NotImplementedException();
        }
    }
}