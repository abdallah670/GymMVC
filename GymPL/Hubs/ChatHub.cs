using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace GymPL.Hubs
{
    public class ChatHub : Hub
    {
        // Track number of connections per user
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, int> OnlineUsers = new();

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                OnlineUsers.AddOrUpdate(userId, 1, (key, count) => count + 1);
                
                // If this is the first connection, notify others that user is online
                // Note: In a real app, you might want to debounce this or check if count was 0
                if (OnlineUsers[userId] == 1) 
                {
                    await Clients.All.SendAsync("UserIsOnline", userId);
                }
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                OnlineUsers.AddOrUpdate(userId, 0, (key, count) => Math.Max(0, count - 1));

                if (OnlineUsers.TryGetValue(userId, out var count) && count == 0)
                {
                    OnlineUsers.TryRemove(userId, out _);
                    await Clients.All.SendAsync("UserIsOffline", userId);
                }
            }
            await base.OnDisconnectedAsync(exception);
        }

        public bool IsUserOnline(string userId)
        {
            return OnlineUsers.ContainsKey(userId);
        }
        public async Task SendMessage(int messageId, string receiverId, string message, string? attachmentUrl, string? attachmentType, string? senderName, string? senderPicture)
        {
            var senderId = Context.UserIdentifier;
            var timestamp = DateTime.UtcNow;

            // Send to the specific user (receiver)
            await Clients.User(receiverId).SendAsync("ReceiveMessage", senderId, message, timestamp, attachmentUrl, attachmentType, senderName, senderPicture, messageId);
            
            // Send back to the sender (to sync across multiple devices)
            await Clients.User(senderId!).SendAsync("ReceiveMessage", senderId, message, timestamp, attachmentUrl, attachmentType, senderName, senderPicture, messageId);
        }

        public async Task MessageDelivered(int messageId, string senderId)
        {
            // Notify the original sender
            await Clients.User(senderId).SendAsync("MessageDelivered", messageId);
        }

        public async Task MessageRead(int messageId, string senderId)
        {
            // Notify the original sender (to turn checks blue)
            await Clients.User(senderId).SendAsync("MessageRead", messageId);
            
            // Also notify the current user (receiver) on other devices/tabs to clear badges
            var receiverId = Context.UserIdentifier;
            if (receiverId != null)
            {
                await Clients.User(receiverId).SendAsync("MessageRead", messageId);
            }
        }

        public async Task DeleteMessage(int messageId, string otherUserId)
        {
            // Notify other user
            await Clients.User(otherUserId).SendAsync("MessageDeleted", messageId);
            // Notify self on other tabs/devices
            var currentUserId = Context.UserIdentifier;
            if (currentUserId != null)
            {
                 await Clients.User(currentUserId).SendAsync("MessageDeleted", messageId);
            }
        }

        public async Task JoinChat(string otherUserId)
        {
            // Logic for joining a specific chat session could go here
        }
    }
}
