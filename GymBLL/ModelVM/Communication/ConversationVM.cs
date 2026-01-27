using System;

namespace GymBLL.ModelVM.Communication
{
    public class ConversationVM
    {
        public string OtherUserId { get; set; }
        public string OtherUserName { get; set; }
        public string? OtherUserPicture { get; set; }
        public string LastMessage { get; set; }
        public DateTime LastMessageTimestamp { get; set; }
        public int UnreadCount { get; set; }
        public string? LastMessageAttachmentType { get; set; }
    }
}
