using Health.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Graduation_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        // api/Chat
        [HttpGet]
        public async Task<IActionResult> GetAllChats([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var chats = await _chatService.GetAllChatsAsync(currentUserId, pageNumber, pageSize);
            return Ok(chats);
        }

        //api/Chat/{otherUserId}/history
        [HttpGet("{otherUserId}/history")]
        public async Task<IActionResult> GetChatHistory(Guid otherUserId)
        {
            var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var messages = await _chatService.GetChatHistoryAsync(currentUserId, otherUserId);
            return Ok(messages);
        }

        //api/Chat/{otherUserId}/read
        [HttpPut("{otherUserId}/read")]
        public async Task<IActionResult> MarkAsRead(Guid otherUserId)
        {
            var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _chatService.MarkMessagesAsReadAsync(currentUserId, otherUserId);
            return Ok(new { Message = "Messages marked as read." });
        }
    }
}
