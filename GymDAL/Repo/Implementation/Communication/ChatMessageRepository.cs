using GymDAL.DB;
using GymDAL.Entities.Communication;
using GymDAL.Repo.Abstract.Communication;
using GymDAL.Repo.Implementation;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymDAL.Repo.Implementation.Communication
{
    public class ChatMessageRepository : Repository<ChatMessage>, IChatMessageRepository
    {
        public ChatMessageRepository(GymDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ChatMessage>> GetChatHistoryPagedAsync(string user1Id, string user2Id, int skip, int take)
        {
            return await _context.ChatMessages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => m.DeletedAt == null && ((m.SenderId == user1Id && m.ReceiverId == user2Id) || 
                            (m.SenderId == user2Id && m.ReceiverId == user1Id)))
                .OrderByDescending(m => m.Timestamp)
                .Skip(skip)
                .Take(take)
                .Reverse() // Keep in chronological order for the view
                .ToListAsync();
        }

        public async Task<IEnumerable<ChatMessage>> GetUnreadMessagesAsync(string userId)
        {
            return await _context.ChatMessages
                .Include(m => m.Sender)
                .Where(m => m.DeletedAt == null && m.ReceiverId == userId && !m.IsRead)
                .ToListAsync();
        }

        public async Task<IEnumerable<ChatMessage>> GetConversationsWithUsersAsync(string userId)
        {
            return await _context.ChatMessages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => m.DeletedAt == null &&(m.SenderId == userId || m.ReceiverId == userId))
                .OrderByDescending(m => m.Timestamp)
                .ToListAsync();
        }
    }
}
