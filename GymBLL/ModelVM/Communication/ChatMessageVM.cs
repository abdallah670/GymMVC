using System;

namespace GymBLL.ModelVM.Communication
{
    public class ChatMessageVM
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string ReceiverId { get; set; }
        public string ReceiverName { get; set; }
        public string? Message { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }
        public bool IsDelivered { get; set; }
        public string? AttachmentUrl { get; set; }
        public string? AttachmentType { get; set; }
        public string? SenderPicture { get; set; }
        public string? ReceiverPicture { get; set; }
    }
}
