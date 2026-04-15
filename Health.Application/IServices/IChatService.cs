using Health.Application.Models;
using Health.Contracts.Requests.Chat;
using Health.Contracts.Responses.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.IServices
{
    public interface IChatService
    {
        Task<MessageResponse> SendMessageAsync(Guid senderId, SendMessageRequest request);
        Task<List<MessageResponse>> GetChatHistoryAsync(Guid currentUserId, Guid otherUserId);
        Task<List<ChatResponse>> GetAllChatsAsync(Guid currentUserId);
        Task MarkMessagesAsReadAsync(Guid currentUserId, Guid otherUserId);
        Task<Chat> GetOrCreateChatAsync(Guid firstUserId, Guid secondUserId);
    }
}
