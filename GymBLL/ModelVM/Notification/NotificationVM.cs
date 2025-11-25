using System;
using System.ComponentModel.DataAnnotations;

namespace GymBLL.ModelVM.Notification
{
    public class NotificationVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "User is required")]
        public string UserId { get; set; }

        public string? UserName { get; set; }

        public string Type { get; set; } = "General";

        [Required(ErrorMessage = "Message is required")]
        [StringLength(500)]
        public string Message { get; set; }

        public string Status { get; set; } = "Unread";

        public DateTime SendTime { get; set; } = DateTime.UtcNow;

        public string? DeliveryMethod { get; set; }

        [StringLength(100)]
        public string? RelatedEntity { get; set; }

        public int? RelatedEntityId { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
