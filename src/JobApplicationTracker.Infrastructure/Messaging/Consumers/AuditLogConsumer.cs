using JobApplicationTracker.Application.Common.Interfaces;
using JobApplicationTracker.Domain.Entities;
using JobApplicationTracker.Domain.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace JobApplicationTracker.Infrastructure.Messaging.Consumers;

public class AuditLogConsumer : IConsumer<ApplicationStatusChangedEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<AuditLogConsumer> _logger;

    public AuditLogConsumer(IApplicationDbContext context, ILogger<AuditLogConsumer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ApplicationStatusChangedEvent> context)
    {
        var message = context.Message;

        var entry = new AuditLogEntry
        {
            ApplicationId = message.ApplicationId,
            FromStatus = message.FromStatus,
            ToStatus = message.ToStatus,
            ChangedByUserId = message.UserId
        };

        _context.AuditLogs.Add(entry);
        await _context.SaveChangesAsync(context.CancellationToken);

        _logger.LogInformation(
            "Audit log recorded for application {ApplicationId}: {FromStatus} -> {ToStatus}",
            message.ApplicationId, message.FromStatus, message.ToStatus);
    }
}
