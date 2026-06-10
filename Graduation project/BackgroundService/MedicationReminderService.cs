using Graduation_project.Hubs;
using Health.Application.IServices;
using Health.Application.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Graduation_project.BackgroundServices
{
    public class MedicationReminderService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<MedicationReminderService> _logger;

        // How often the service checks (every 1 minute)
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

        public MedicationReminderService(
            IServiceScopeFactory scopeFactory,
            IHubContext<NotificationHub> hubContext,
            ILogger<MedicationReminderService> logger)
        {
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Medication Reminder Service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAndSendRemindersAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in MedicationReminderService.");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task CheckAndSendRemindersAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<WateenDbContext>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            var now = DateTime.UtcNow;

            // Find medications due right now
            var dueMedications = await dbContext.Medications
                .Include(m => m.Patient).ThenInclude(p => p.User)
                .Where(m => m.IsActive
                         && m.NextReminderTime.HasValue
                         && m.NextReminderTime.Value <= now
                         && (m.EndDate == null || m.NextReminderTime.Value <= m.EndDate))
                .ToListAsync();

            foreach (var medication in dueMedications)
            {
                var userId = medication.Patient.User.Id;
                var title = $"💊 Time to take {medication.Name}";
                var message = $"Dosage: {medication.Dosage}. {medication.Instructions}";

                // Send the real-time notification
                var notification = await notificationService.CreateNotificationAsync(userId, title, message, "reminder");

                if (NotificationHub.OnlineUsers.TryGetValue(userId.ToString(), out var connectionId))
                {
                    await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveNotification", new
                    {
                        notification.Id,
                        notification.Title,
                        notification.Message,
                        notification.Type,
                        notification.IsRead,
                        notification.CreatedAt
                    });
                }

                // Schedule the next reminder using your integer Frequency
                medication.NextReminderTime = medication.NextReminderTime.Value.AddHours(medication.Frequency);

                // Auto-complete if it passes the EndDate
                if (medication.EndDate.HasValue && medication.NextReminderTime > medication.EndDate.Value)
                {
                    medication.NextReminderTime = null;
                    medication.IsActive = false;
                }
            }

            if (dueMedications.Any())
            {
                await dbContext.SaveChangesAsync();
            }
        }
    }
}