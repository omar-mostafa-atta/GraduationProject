using Health.Application.IServices;
using Health.Application.Models;
using Health.Contracts.Common;
using Health.Contracts.Requests.Chat;
using Health.Contracts.Responses.Chat;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.Services
{
    public class ChatService : IChatService
    {
        private readonly WateenDbContext _dbContext;

        public ChatService(WateenDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Chat> GetOrCreateChatAsync(Guid firstUserId, Guid secondUserId)
        {
            // 
            var (smallerId, largerId) = firstUserId < secondUserId
                ? (firstUserId, secondUserId)
                : (secondUserId, firstUserId);

            var chat = await _dbContext.Chats
                .FirstOrDefaultAsync(c =>
                    c.FirstUserId == smallerId && c.SecondUserId == largerId);

            if (chat == null)
            {
                chat = new Chat
                {
                    Id = Guid.NewGuid(),
                    FirstUserId = smallerId,
                    SecondUserId = largerId
                };
                _dbContext.Chats.Add(chat);
                await _dbContext.SaveChangesAsync();
            }

            return chat;
        }

        public async Task<MessageResponse> SendMessageAsync(Guid senderId, SendMessageRequest request)
        {
            var chat = await GetOrCreateChatAsync(senderId, request.ReceiverId);

            var message = new Message
            {
                Id = Guid.NewGuid(),
                SenderId = senderId,
                ReceiverId = request.ReceiverId,
                ChatId = chat.Id,
                MessageContent = request.MessageContent,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            _dbContext.Messages.Add(message);
            await _dbContext.SaveChangesAsync();

            var sender = await _dbContext.Users.FindAsync(senderId);

            return MapToMessageResponse(message, sender);
        }

        public async Task<List<MessageResponse>> GetChatHistoryAsync(Guid currentUserId, Guid otherUserId)
        {
            var chat = await GetOrCreateChatAsync(currentUserId, otherUserId);

            var messages = await _dbContext.Messages
                .Where(m => m.ChatId == chat.Id)
                .Include(m => m.Sender)
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            return messages.Select(m => MapToMessageResponse(m, m.Sender)).ToList();
        }

        public async Task<PaginatedResponse<ChatResponse>> GetAllChatsAsync(Guid currentUserId, int pageNumber, int pageSize)
        {
            var totalCount = await _dbContext.Chats
                .Where(c => c.FirstUserId == currentUserId || c.SecondUserId == currentUserId)
                .CountAsync();

            var chats = await _dbContext.Chats
                .Where(c => c.FirstUserId == currentUserId || c.SecondUserId == currentUserId)
                .Include(c => c.Messages)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new List<ChatResponse>();

            foreach (var chat in chats)
            {
                var otherUserId = chat.FirstUserId == currentUserId
                    ? chat.SecondUserId
                    : chat.FirstUserId;

                var otherUser = await _dbContext.Users.FindAsync(otherUserId);
                var lastMessage = chat.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault();
                var unreadCount = chat.Messages.Count(m => m.ReceiverId == currentUserId && !m.IsRead);

                result.Add(new ChatResponse
                {
                    Id = chat.Id,
                    OtherUserId = otherUserId,
                    OtherUserName = $"{otherUser?.FirstName} {otherUser?.LastName}",
                    OtherUserProfilePicture = otherUser?.ProfilePictureUrl,
                    LastMessage = lastMessage != null ? MapToMessageResponse(lastMessage, null) : null,
                    UnreadCount = unreadCount
                });
            }

            return new PaginatedResponse<ChatResponse>
            {
                Data = result.OrderByDescending(c => c.LastMessage?.SentAt).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task MarkMessagesAsReadAsync(Guid currentUserId, Guid otherUserId)
        {
            var chat = await GetOrCreateChatAsync(currentUserId, otherUserId);

            var unreadMessages = await _dbContext.Messages
                .Where(m => m.ChatId == chat.Id && m.ReceiverId == currentUserId && !m.IsRead)
                .ToListAsync();

            unreadMessages.ForEach(m => m.IsRead = true);
            await _dbContext.SaveChangesAsync();
        }

        private MessageResponse MapToMessageResponse(Message message, User sender)
        {
            return new MessageResponse
            {
                Id = message.Id,
                SenderId = message.SenderId,
                SenderName = sender != null ? $"{sender.FirstName} {sender.LastName}" : null,
                SenderProfilePicture = sender?.ProfilePictureUrl,
                ReceiverId = message.ReceiverId,
                ChatId = message.ChatId,
                MessageContent = message.MessageContent,
                SentAt = message.SentAt,
                IsRead = message.IsRead
            };
        }
    }

}
