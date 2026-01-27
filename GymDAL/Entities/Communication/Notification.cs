using GymDAL.Entities.Users;
using GymDAL.Entities.Core;
using GymDAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymDAL.Entities.Communication
{
    public class Notification : BaseEntity
    {
        public int Id { get; set; }
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

        public string? RelatedEntityId { get; set; }

        // Navigation properties
        public virtual ApplicationUser User { get; set; }
        public bool ToggleStatus()
        {
           
                this.IsActive = !this.IsActive;
             
                return true;
            
        }
    }

}
