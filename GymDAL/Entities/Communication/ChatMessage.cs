using GymDAL.Entities.Core;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymDAL.Entities.Communication
{
    public class ChatMessage : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string SenderId { get; set; }

        [Required]
        public string ReceiverId { get; set; }

        [StringLength(2000)]
        public string? Message { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;
        
        public bool IsDelivered { get; set; } = false;
        
        public string? AttachmentUrl { get; set; }
        
        public string? AttachmentType { get; set; } // image, video, file

       // Navigation properties(if needed, assuming ApplicationUser exists)
         [ForeignKey("SenderId")]
        public virtual ApplicationUser Sender { get; set; }

        [ForeignKey("ReceiverId")]
        public virtual ApplicationUser Receiver { get; set; }
    }
}
