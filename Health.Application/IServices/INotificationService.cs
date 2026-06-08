using Health.Application.Models;
using Health.Contracts.Common;
using Health.Contracts.Responses.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.IServices
{
    public interface INotificationService
    {
        Task<Notification> CreateNotificationAsync(Guid userId, string title, string message, string type);
        Task<PaginatedResponse<NotificationResponse>> GetMyNotificationsAsync(string userId, bool? isRead, int pageNumber, int pageSize);
        Task MarkAsReadAsync(string userId, Guid notificationId);
        Task MarkAllAsReadAsync(string userId);
    }
}
