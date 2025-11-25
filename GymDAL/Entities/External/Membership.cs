using System.ComponentModel.DataAnnotations;
using GymDAL.Entities.Users;

namespace GymDAL.Entities
{
    public class Membership : BaseEntity
    {
        [Required]
        public string MemberId { get; set; }

        public string? TrainerId { get; set; } // Associated trainer for personal training

        [Required]
        [StringLength(50)]
        public string MembershipType { get; set; } // "Basic", "Premium", "VIP", "Student", "Corporate", "PersonalTraining"

        [Required]
        [StringLength(100)]
        public string PlanName { get; set; } // "Monthly Basic", "Annual Premium", "Personal Training Package", etc.

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [Required]
        [StringLength(20)]
        public string BillingCycle { get; set; } // "Monthly", "Quarterly", "Annual", "OneTime"

        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime EndDate { get; set; }
        public DateTime? RenewalDate { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Active"; // "Active", "Expired", "Cancelled", "Suspended"

        [StringLength(20)]
        public string PaymentStatus { get; set; } = "Paid"; // "Paid", "Pending", "Overdue"
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
        // Training-specific properties
        public int? TotalSessions { get; set; } // Total sessions in package
        public int? UsedSessions { get; set; } = 0; // Sessions used so far
        public int? SessionsPerWeek { get; set; } // Number of allowed sessions per week
        public int? SessionDuration { get; set; } // Session duration in minutes

        // Access and benefits
        public bool HasPersonalTraining { get; set; } = false;
        public bool HasGroupClasses { get; set; } = false;
        public bool HasDietPlan { get; set; } = false;
        public bool HasPoolAccess { get; set; } = false;
        public bool HasSaunaAccess { get; set; } = false;
        public bool HasWorkoutPlan { get; set; } = false;
        public bool HasLocker { get; set; } = false;
        public bool HasTowelService { get; set; } = false;

        [StringLength(500)]
        public string? Benefits { get; set; } // JSON or comma-separated benefits

        [StringLength(1000)]
        public string? TermsAndConditions { get; set; }

        public DateTime? CancellationDate { get; set; }
        
        [StringLength(500)]
        public string? CancellationReason { get; set; }

        public int? MaxFreezeDays { get; set; } // Maximum days member can freeze membership
        public int? UsedFreezeDays { get; set; } = 0;

        // Training schedule preferences
        [StringLength(50)]
        public string? PreferredTrainingTime { get; set; } // "Morning", "Afternoon", "Evening"
        
        [StringLength(20)]
        public string? TrainingIntensity { get; set; } // "Light", "Moderate", "High"
                                                       // Associated Plans
        public int? WorkoutPlanId { get; set; } // Associated workout plan
        public int? DietPlanId { get; set; } // Associated diet plan

        // Navigation properties
        public virtual Member Member { get; set; }
        public virtual Trainer Trainer { get; set; } // Associated trainer
        public virtual WorkoutPlan WorkoutPlan { get; set; } // Associated workout plan
        public virtual DietPlan DietPlan { get; set; } // Associated diet plan
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<TrainingSession> TrainingSessions { get; set; } = new List<TrainingSession>();
    }
}