using AutoMapper;
using GymBLL.ModelVM.Communication;
using GymBLL.Response;
using GymBLL.Service.Abstract;
using GymBLL.Service.Abstract.Communication;
using GymDAL.Entities.Communication;
using GymDAL.Repo.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymBLL.Service.Implementation.Communication
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ChatService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Response<ChatMessageVM>> SendMessageAsync(ChatMessageVM model)
        {
            try
            {
                var message = new ChatMessage
                {
                    SenderId = model.SenderId,
                    ReceiverId = model.ReceiverId,
                    Message = model.Message,
                    Timestamp = DateTime.UtcNow,
                    IsRead = false,
                    IsDelivered = false,
                    AttachmentUrl = model.AttachmentUrl,
                    AttachmentType = model.AttachmentType
                };

                await _unitOfWork.ChatMessages.AddAsync(message);
                var result = await _unitOfWork.SaveAsync();

                if (result > 0)
                {
                    model.Id = message.Id;
                    model.Timestamp = message.Timestamp;

                    // Populate additional fields for real-time updates
                    var sender = await _unitOfWork.ApplicationUsers.GetByIdAsync(message.SenderId);
                    var receiver = await _unitOfWork.ApplicationUsers.GetByIdAsync(message.ReceiverId);

                    if (sender != null)
                    {
                        model.SenderName = sender.FullName;
                        model.SenderPicture = sender.ProfilePicture;
                    }
                    if (receiver != null)
                    {
                        model.ReceiverName = receiver.FullName;
                        model.ReceiverPicture = receiver.ProfilePicture;
                    }

                    return new Response<ChatMessageVM>(model, null, false);
                }
                return new Response<ChatMessageVM>(null, "Failed to send message.", true);
            }
            catch (Exception ex)
            {
                return new Response<ChatMessageVM>(null, $"Error: {ex.Message}", true);
            }
        }

        public async Task<Response<List<ChatMessageVM>>> GetChatHistoryAsync(string userId, string otherUserId, int skip = 0, int take = 50)
        {
            try
            {
                var history = await _unitOfWork.ChatMessages.GetChatHistoryPagedAsync(userId, otherUserId, skip, take);
                var vms = history.Select(m => _mapper.Map<ChatMessageVM>(m)).ToList();
                return new Response<List<ChatMessageVM>>(vms, null, false);
            }
            catch (Exception ex)
            {
                return new Response<List<ChatMessageVM>>(null, $"Error: {ex.Message}", true);
            }
        }

        public async Task<Response<List<ChatMessageVM>>> GetUnreadMessagesAsync(string userId)
        {
            try
            {
                var unread = await _unitOfWork.ChatMessages.GetUnreadMessagesAsync(userId);
                var vms = unread.Select(m => _mapper.Map<ChatMessageVM>(m)).ToList();
                return new Response<List<ChatMessageVM>>(vms, null, false);
            }
            catch (Exception ex)
            {
                return new Response<List<ChatMessageVM>>(null, $"Error: {ex.Message}", true);
            }
        }

        public async Task<Response<bool>> MarkAsReadAsync(int messageId)
        {
            try
            {
                var message = await _unitOfWork.ChatMessages.GetByIdAsync(messageId);
                if (message == null) return new Response<bool>(false, "Message not found.", true);

                message.IsRead = true;
                _unitOfWork.ChatMessages.Update(message);
                var result = await _unitOfWork.SaveAsync();

                return new Response<bool>(result > 0, null, result <= 0);
            }
            catch (Exception ex)
            {
                return new Response<bool>(false, $"Error: {ex.Message}", true);
            }
        }

        public async Task<Response<bool>> MarkAsDeliveredAsync(int messageId)
        {
            try
            {
                var message = await _unitOfWork.ChatMessages.GetByIdAsync(messageId);
                if (message == null) return new Response<bool>(false, "Message not found.", true);

                if (message.IsDelivered) return new Response<bool>(true, null, false);

                message.IsDelivered = true;
                _unitOfWork.ChatMessages.Update(message);
                var result = await _unitOfWork.SaveAsync();

                return new Response<bool>(result > 0, null, result <= 0);
            }
            catch (Exception ex)
            {
                return new Response<bool>(false, $"Error: {ex.Message}", true);
            }
        }

        public async Task<Response<bool>> DeleteMessageAsync(int messageId, string userId)
        {
            try
            {
                var message = await _unitOfWork.ChatMessages.GetByIdAsync(messageId);
                if (message == null) return new Response<bool>(false, "Message not found.", true);

                // Ensure only the sender can delete
                if (message.SenderId != userId)
                {
                    return new Response<bool>(false, "You can only delete your own messages.", true);
                }

                // Soft delete
                message.DeletedAt = DateTime.UtcNow;
                _unitOfWork.ChatMessages.Update(message);
                
                var result = await _unitOfWork.SaveAsync();
                return new Response<bool>(result > 0, null, result <= 0);
            }
            catch (Exception ex)
            {
                return new Response<bool>(false, $"Error: {ex.Message}", true);
            }
        }

        public async Task<Response<List<ConversationVM>>> GetRecentConversationsAsync(string userId)
        {
            try
            {
                var messages = await _unitOfWork.ChatMessages.GetConversationsWithUsersAsync(userId);
                
                // Group by conversation partner
                var conversations = messages
                    .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                    .OrderByDescending(m => m.Timestamp)
                    .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                    .Select(g => new ConversationVM
                    {
                        OtherUserId = g.Key,
                        OtherUserName = g.First().SenderId == userId ? g.First().Receiver?.FullName : g.First().Sender?.FullName,
                        OtherUserPicture = g.First().SenderId == userId ? g.First().Receiver?.ProfilePicture : g.First().Sender?.ProfilePicture,
                        LastMessage = g.First().Message,
                        LastMessageTimestamp = g.First().Timestamp,
                        LastMessageAttachmentType = g.First().AttachmentType,
                        UnreadCount = g.Count(m => m.ReceiverId == userId && !m.IsRead)
                    })
                    .ToList();

                return new Response<List<ConversationVM>>(conversations, null, false);
            }
            catch (Exception ex)
            {
                return new Response<List<ConversationVM>>(null, $"Error: {ex.Message}", true);
            }
        }
    }
}
