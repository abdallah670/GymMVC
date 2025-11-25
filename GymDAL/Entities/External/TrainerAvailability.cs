using System.ComponentModel.DataAnnotations;
using GymDAL.Entities.Users;

namespace GymDAL.Entities
{
    public class TrainerAvailability : BaseEntity
    {
        [Required]
        public string TrainerId { get; set; }

        [Required]
        public DayOfWeek DayOfWeek { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [Required]
        [StringLength(20)]
        public string AvailabilityType { get; set; } = "Regular"; // "Regular", "Holiday", "TimeOff"

        public bool IsAvailable { get; set; } = true;

        [StringLength(200)]
        public string? Notes { get; set; }

        public DateRange? SpecificDateRange { get; set; } // For temporary availability changes

        // Navigation properties
        public virtual Trainer Trainer { get; set; }
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

    // Complex type for date range
    [ComplexType]
    public class DateRange
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}