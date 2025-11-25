using System.ComponentModel.DataAnnotations;
using GymDAL.Entities.Users;

namespace GymDAL.Entities.Progress_and_Notification
{
    public class SystemLog : BaseEntity
    {
        [Required]
        public string AdminId { get; set; } // Who performed the action

        [Required]
        [StringLength(100)]
        public string Action { get; set; } // "CreateUser", "DeleteTrainer", "UpdateSystemSettings"

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(50)]
        public string EntityType { get; set; } // "User", "Trainer", "Member", "System"

        public string? EntityId { get; set; } // ID of the affected entity

        public DateTime LogDate { get; set; } = DateTime.UtcNow;

        [StringLength(20)]
        public string LogLevel { get; set; } = "Info"; // "Info", "Warning", "Error"

        public string? OldValues { get; set; } // JSON of old values
        public string? NewValues { get; set; } // JSON of new values

        [StringLength(1000)]
        public string? Notes { get; set; }

        // Navigation properties
        public virtual Admin Admin { get; set; }
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