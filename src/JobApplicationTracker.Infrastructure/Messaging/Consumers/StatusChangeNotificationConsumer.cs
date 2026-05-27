using JobApplicationTracker.Application.Common.Interfaces;
using JobApplicationTracker.Domain.Entities;
using JobApplicationTracker.Domain.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace JobApplicationTracker.Infrastructure.Messaging.Consumers;

public class StatusChangeNotificationConsumer : IConsumer<ApplicationStatusChangedEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly INotificationPusher _notificationPusher;
    private readonly ILogger<StatusChangeNotificationConsumer> _logger;

    public StatusChangeNotificationConsumer(IApplicationDbContext context, INotificationPusher notificationPusher, ILogger<StatusChangeNotificationConsumer> logger)
    {
        _context = context;
        _notificationPusher = notificationPusher;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ApplicationStatusChangedEvent> context)
    {
        var message = context.Message;

        var notification = new Notification
        {
            UserId = message.UserId,
            Title = "Application status updated",
            Message = $"Status changed from {message.FromStatus} to {message.ToStatus}.",
            RelatedApplicationId = message.ApplicationId
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync(context.CancellationToken);

        await _notificationPusher.PushNotificationAsync(
            notification.UserId, notification.Title, notification.Message, notification.RelatedApplicationId, context.CancellationToken);

        _logger.LogInformation(
            "Notification created for user {UserId} regarding application {ApplicationId}",
            message.UserId, message.ApplicationId);
    }
}