using GymDAL.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymDAL.Entities.Progress_and_Notification
{
    public class Notification : BaseEntity
    {
        [Required]
        public string UserId { get; set; }

        public NotificationType Type { get; set; }

        [Required]
        [StringLength(500)]
        public string Message { get; set; }

        public NotificationStatus Status { get; set; } = NotificationStatus.Unread;

        public DateTime SendTime { get; set; } = DateTime.UtcNow;

        public DeliveryMethod? DeliveryMethod { get; set; }

        [StringLength(100)]
        public string? RelatedEntity { get; set; } // "WorkoutPlan", "DietPlan", etc.

        public int? RelatedEntityId { get; set; }

        // Navigation properties
        public virtual ApplicationUser User { get; set; }
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
