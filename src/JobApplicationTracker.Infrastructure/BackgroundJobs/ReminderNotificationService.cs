using JobApplicationTracker.Application.Common.Interfaces;
using JobApplicationTracker.Domain.Entities;
using JobApplicationTracker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JobApplicationTracker.Infrastructure.BackgroundJobs;

public class ReminderNotificationService : BackgroundService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromMinutes(1);
    private static readonly TimeSpan DueWindow = TimeSpan.FromMinutes(5);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ReminderNotificationService> _logger;

    public ReminderNotificationService(IServiceScopeFactory scopeFactory, ILogger<ReminderNotificationService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await NotifyDueRemindersAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while checking for due reminders");
            }

            await Task.Delay(PollInterval, stoppingToken);
        }
    }

    private async Task NotifyDueRemindersAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var notificationPusher = scope.ServiceProvider.GetRequiredService<INotificationPusher>();

        var cutoff = DateTime.UtcNow.Add(DueWindow);

        var dueReminders = await context.Reminders
            .IgnoreQueryFilters()
            .Include(r => r.Application)
            .Where(r => !r.IsCompleted && r.NotifiedAtUtc == null && r.DueDateUtc <= cutoff)
            .ToListAsync(cancellationToken);

        if (dueReminders.Count == 0)
        {
            return;
        }

        var notifications = new List<Notification>();

        foreach (var reminder in dueReminders)
        {
            if (reminder.Application is null)
            {
                continue;
            }

            var notification = new Notification
            {
                UserId = reminder.Application.UserId,
                Title = "Reminder due",
                Message = $"\"{reminder.Title}\" is due {FormatDueDate(reminder.DueDateUtc)}.",
                RelatedApplicationId = reminder.ApplicationId
            };

            context.Notifications.Add(notification);
            notifications.Add(notification);

            reminder.NotifiedAtUtc = DateTime.UtcNow;
        }

        await context.SaveChangesAsync(cancellationToken);

        foreach (var notification in notifications)
        {
            await notificationPusher.PushNotificationAsync(
                notification.UserId, notification.Title, notification.Message, notification.RelatedApplicationId, cancellationToken);
        }

        _logger.LogInformation("Created {Count} reminder due notification(s)", notifications.Count);
    }

    private static string FormatDueDate(DateTime dueDateUtc)
    {
        var day = dueDateUtc.Day;
        var suffix = (day % 10, day) switch
        {
            (_, 11) or (_, 12) or (_, 13) => "th",
            (1, _) => "st",
            (2, _) => "nd",
            (3, _) => "rd",
            _ => "th"
        };

        return dueDateUtc.ToString($"d'{suffix}' MMMM yyyy, HH:mm");
    }
}
