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
            var todayUtc = now.Date;

            // Load all active medications with patient user info
            var medications = await dbContext.Medications
                .Include(m => m.Patient)
                    .ThenInclude(p => p.User)
                .Where(m => m.IsActive
                    && m.StartDate.Date <= todayUtc
                    && (m.EndDate == null || m.EndDate.Value.Date >= todayUtc))
                .ToListAsync();

            foreach (var medication in medications)
            {
                var reminderTimes = GetReminderTimes(medication.Frequency); // e.g. [8, 20]

                foreach (var hour in reminderTimes)
                {
                    // Check if current hour matches and we're within the first minute of that hour
                    if (now.Hour != hour || now.Minute != 0)
                        continue;

                    // Avoid duplicate: check if we already sent this reminder today
                    var alreadySent = await dbContext.Notifications.AnyAsync(n =>
                        n.UserId == medication.Patient.User.Id
                        && n.Type == "reminder"
                        && n.Title.Contains(medication.Name)
                        && n.CreatedAt.Date == todayUtc
                        && n.CreatedAt.Hour == hour);

                    if (alreadySent) continue;

                    var userId = medication.Patient.User.Id;
                    var title = $"💊 Time to take {medication.Name}";
                    var message = $"Dosage: {medication.Dosage}. {medication.Instructions}";

                    // Save to DB
                    var notification = await notificationService
                        .CreateNotificationAsync(userId, title, message, "reminder");

                    // Push real-time via SignalR if user is online
                    var userIdStr = userId.ToString();
                    if (NotificationHub.OnlineUsers.TryGetValue(userIdStr, out var connectionId))
                    {
                        await _hubContext.Clients.Client(connectionId)
                            .SendAsync("ReceiveNotification", new
                            {
                                notification.Id,
                                notification.Title,
                                notification.Message,
                                notification.Type,
                                notification.IsRead,
                                notification.CreatedAt
                            });
                    }

                    _logger.LogInformation(
                        "Sent medication reminder to user {UserId} for {MedicationName}",
                        userId, medication.Name);
                }
            }
        }

        /// <summary>
        /// Returns UTC hours when reminders should fire based on frequency string.
        /// Adjust these hours to match your users' timezone if needed.
        /// </summary>
        private static List<int> GetReminderTimes(string frequency)
        {
            return frequency?.ToLower() switch
            {
                "once daily" => new List<int> { 8 },
                "twice daily" => new List<int> { 8, 20 },
                "three times" => new List<int> { 8, 14, 20 },
                "every 8 hours" => new List<int> { 8, 16, 0 },
                "every 12 hours" => new List<int> { 8, 20 },
                _ => new List<int> { 8 } // default: morning
            };
        }
    }
}