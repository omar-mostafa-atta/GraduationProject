using Health.Application.IServices;
using Health.Application.Models;
using Health.Contracts.Common;
using Health.Contracts.Responses.Notification;
using Microsoft.EntityFrameworkCore;

namespace Health.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly WateenDbContext _dbContext;

        public NotificationService(WateenDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Notification> CreateNotificationAsync(Guid userId, string title, string message, string type)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Notifications.Add(notification);
            await _dbContext.SaveChangesAsync();
            return notification;
        }

        public async Task<PaginatedResponse<NotificationResponse>> GetMyNotificationsAsync(
            string userId, bool? isRead, int pageNumber, int pageSize)
        {
            if (!Guid.TryParse(userId, out var userGuid))
                throw new Exception("Invalid User ID.");

            var query = _dbContext.Notifications
                .Where(n => n.UserId == userGuid);

            if (isRead.HasValue)
                query = query.Where(n => n.IsRead == isRead.Value);

            var totalCount = await query.CountAsync();

            var notifications = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<NotificationResponse>
            {
                Data = notifications.Select(MapToResponse).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task MarkAsReadAsync(string userId, Guid notificationId)
        {
            if (!Guid.TryParse(userId, out var userGuid))
                throw new Exception("Invalid User ID.");

            var notification = await _dbContext.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userGuid);

            if (notification == null)
                throw new Exception("Notification not found.");

            notification.IsRead = true;
            await _dbContext.SaveChangesAsync();
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
            if (!Guid.TryParse(userId, out var userGuid))
                throw new Exception("Invalid User ID.");

            var notifications = await _dbContext.Notifications
                .Where(n => n.UserId == userGuid && !n.IsRead)
                .ToListAsync();

            notifications.ForEach(n => n.IsRead = true);
            await _dbContext.SaveChangesAsync();
        }

        private NotificationResponse MapToResponse(Notification n) => new()
        {
            Id = n.Id,
            Title = n.Title,
            Message = n.Message,
            Type = n.Type,
            IsRead = n.IsRead,
            CreatedAt = n.CreatedAt
        };
    }
}