// Graduation_project/Controllers/NotificationController.cs
using Graduation_project.Hubs;
using Health.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Graduation_project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // GET: api/Notification/my?isRead=false
        [HttpGet("my")]
        public async Task<IActionResult> GetMyNotifications(
            [FromQuery] bool? isRead,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            try
            {
                var result = await _notificationService.GetMyNotificationsAsync(userId, isRead, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }

        // PUT: api/Notification/read/{notificationId}
        [HttpPut("read/{notificationId}")]
        public async Task<IActionResult> MarkAsRead(Guid notificationId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            try
            {
                await _notificationService.MarkAsReadAsync(userId, notificationId);
                return Ok(new { Message = "Notification marked as read." });
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }

        // PUT: api/Notification/read-all
        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            try
            {
                await _notificationService.MarkAllAsReadAsync(userId);
                return Ok(new { Message = "All notifications marked as read." });
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }
        [HttpPost("test-send")]
        public async Task<IActionResult> TestSendNotification([FromServices] IHubContext<NotificationHub> hubContext)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdStr == null) return Unauthorized();

            var userId = Guid.Parse(userIdStr);

            // 1. Save to DB
            var notification = await _notificationService.CreateNotificationAsync(
                userId,
                "🧪 Test Alert",
                "If you see this, SignalR is working perfectly!",
                "system");

            // 2. Push to SignalR
            if (NotificationHub.OnlineUsers.TryGetValue(userIdStr, out var connectionId))
            {
                await hubContext.Clients.Client(connectionId).SendAsync("ReceiveNotification", new
                {
                    notification.Id,
                    notification.Title,
                    notification.Message,
                    notification.Type,
                    notification.IsRead,
                    notification.CreatedAt
                });
            }

            return Ok(new { Message = "Test notification fired!" });
        }
    }
}