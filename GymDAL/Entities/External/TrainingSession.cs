using System.ComponentModel.DataAnnotations;
using GymDAL.Entities.Users;

namespace GymDAL.Entities
{
    public class TrainingSession : BaseEntity
    {
        [Required]
        public int MembershipId { get; set; }

        [Required]
        public string TrainerId { get; set; }

        [Required]
        public string MemberId { get; set; }

        [Required]
        public DateTime SessionDate { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [Required]
        [StringLength(50)]
        public string SessionType { get; set; } // "PersonalTraining", "GroupClass", "Consultation"

        [StringLength(100)]
        public string? FocusArea { get; set; } // "Strength", "Cardio", "Flexibility", "WeightLoss"

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Scheduled"; // "Scheduled", "Completed", "Cancelled", "NoShow"

        public bool MemberAttended { get; set; } = false;
        public bool TrainerAttended { get; set; } = false;

        [StringLength(500)]
        public string? SessionNotes { get; set; } // Trainer's notes about the session

        [StringLength(500)]
        public string? MemberFeedback { get; set; } // Member's feedback

        public int? CaloriesBurned { get; set; }
        
        [StringLength(1000)]
        public string? Exercises { get; set; } // JSON of exercises performed

        public DateTime? ActualStartTime { get; set; }
        public DateTime? ActualEndTime { get; set; }

        [StringLength(500)]
        public string? CancellationReason { get; set; }

        // Navigation properties
        public virtual Membership Membership { get; set; }
        public virtual Trainer Trainer { get; set; }
        public virtual Member Member { get; set; }
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
    }
}