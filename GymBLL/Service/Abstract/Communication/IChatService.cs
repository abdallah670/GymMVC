using GymBLL.ModelVM.Communication;
using GymBLL.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymBLL.Service.Abstract.Communication
{
    public interface IChatService
    {
        Task<Response<ChatMessageVM>> SendMessageAsync(ChatMessageVM model);
        Task<Response<List<ChatMessageVM>>> GetChatHistoryAsync(string userId, string otherUserId, int skip = 0, int take = 50);
        Task<Response<List<ChatMessageVM>>> GetUnreadMessagesAsync(string userId);
        Task<Response<bool>> MarkAsReadAsync(int messageId);
        Task<Response<bool>> MarkAsDeliveredAsync(int messageId);
        Task<Response<bool>> DeleteMessageAsync(int messageId, string userId);
        Task<Response<List<ConversationVM>>> GetRecentConversationsAsync(string userId);
    }
}
