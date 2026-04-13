using JobApplicationTracker.Application.Common.Interfaces;
using JobApplicationTracker.Domain.Entities;
using JobApplicationTracker.Domain.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace JobApplicationTracker.Infrastructure.Messaging.Consumers;

public class StatusChangeNotificationConsumer : IConsumer<ApplicationStatusChangedEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<StatusChangeNotificationConsumer> _logger;

    public StatusChangeNotificationConsumer(IApplicationDbContext context, ILogger<StatusChangeNotificationConsumer> logger)
    {
        _context = context;
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

        _logger.LogInformation(
            "Notification created for user {UserId} regarding application {ApplicationId}",
            message.UserId, message.ApplicationId);
    }
}
