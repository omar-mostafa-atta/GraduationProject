using Health.Application.IServices;
using Health.Contracts.Requests.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
namespace Graduation_project.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;
        private static readonly Dictionary<string, string> OnlineUsers = new();

        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                OnlineUsers[userId] = Context.ConnectionId;
                // Notify others this user is online
                await Clients.Others.SendAsync("UserOnline", userId);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                OnlineUsers.Remove(userId);
                // Notify others this user is offline
                await Clients.Others.SendAsync("UserOffline", userId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(SendMessageRequest request)
        {
            var senderIdStr = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (senderIdStr == null) return;

            var senderId = Guid.Parse(senderIdStr);
            var message = await _chatService.SendMessageAsync(senderId, request);

            // Send to sender
            await Clients.Caller.SendAsync("ReceiveMessage", message);

            // Send to receiver if they are online
            var receiverIdStr = request.ReceiverId.ToString();
            if (OnlineUsers.TryGetValue(receiverIdStr, out var receiverConnectionId))
            {
                await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", message);
            }
        }

        public async Task MarkAsRead(Guid otherUserId)
        {
            var currentUserIdStr = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserIdStr == null) return;

            var currentUserId = Guid.Parse(currentUserIdStr);
            await _chatService.MarkMessagesAsReadAsync(currentUserId, otherUserId);

            // Notify the sender their messages were read
            var otherUserIdStr = otherUserId.ToString();
            if (OnlineUsers.TryGetValue(otherUserIdStr, out var connectionId))
            {
                await Clients.Client(connectionId).SendAsync("MessagesRead", currentUserId);
            }
        }

        public Task<bool> IsUserOnline(Guid userId)
        {
            return Task.FromResult(OnlineUsers.ContainsKey(userId.ToString()));
        }
    }
}
