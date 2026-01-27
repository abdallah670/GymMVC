using GymDAL.Entities.Communication;
using GymDAL.Repo.Abstract;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymDAL.Repo.Abstract.Communication
{
    public interface IChatMessageRepository : IRepository<ChatMessage>
    {
        Task<IEnumerable<ChatMessage>> GetChatHistoryPagedAsync(string user1Id, string user2Id, int skip, int take);
        Task<IEnumerable<ChatMessage>> GetUnreadMessagesAsync(string userId);
        Task<IEnumerable<ChatMessage>> GetConversationsWithUsersAsync(string userId);
    }
}
