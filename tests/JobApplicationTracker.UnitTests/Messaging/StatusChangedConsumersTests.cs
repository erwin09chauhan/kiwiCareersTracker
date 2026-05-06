using JobApplicationTracker.Domain.Enums;
using JobApplicationTracker.Domain.Events;
using JobApplicationTracker.Infrastructure.Messaging.Consumers;
using JobApplicationTracker.UnitTests.Common;
using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace JobApplicationTracker.UnitTests.Messaging;

public class StatusChangedConsumersTests
{
    [Fact]
    public async Task AuditLogConsumer_ShouldWriteAuditLogEntry()
    {
        await using var context = TestDbContextFactory.Create();

        await using var provider = new ServiceCollection()
            .AddSingleton(context.GetType(), context)
            .AddSingleton<JobApplicationTracker.Application.Common.Interfaces.IApplicationDbContext>(context)
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<AuditLogConsumer>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();

        try
        {
            var applicationId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            await harness.Bus.Publish(new ApplicationStatusChangedEvent(
                applicationId, userId, ApplicationStatus.Applied, ApplicationStatus.Interview, DateTime.UtcNow));

            (await harness.Consumed.Any<ApplicationStatusChangedEvent>()).Should().BeTrue();

            var consumerHarness = harness.GetConsumerHarness<AuditLogConsumer>();
            (await consumerHarness.Consumed.Any<ApplicationStatusChangedEvent>()).Should().BeTrue();

            var entry = await context.AuditLogs.FirstOrDefaultAsync(a => a.ApplicationId == applicationId);
            entry.Should().NotBeNull();
            entry!.FromStatus.Should().Be(ApplicationStatus.Applied);
            entry.ToStatus.Should().Be(ApplicationStatus.Interview);
            entry.ChangedByUserId.Should().Be(userId);
        }
        finally
        {
            await harness.Stop();
        }
    }

    [Fact]
    public async Task StatusChangeNotificationConsumer_ShouldCreateNotification()
    {
        await using var context = TestDbContextFactory.Create();

        await using var provider = new ServiceCollection()
            .AddSingleton<JobApplicationTracker.Application.Common.Interfaces.IApplicationDbContext>(context)
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<StatusChangeNotificationConsumer>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();

        try
        {
            var applicationId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            await harness.Bus.Publish(new ApplicationStatusChangedEvent(
                applicationId, userId, ApplicationStatus.Applied, ApplicationStatus.Interview, DateTime.UtcNow));

            var consumerHarness = harness.GetConsumerHarness<StatusChangeNotificationConsumer>();
            (await consumerHarness.Consumed.Any<ApplicationStatusChangedEvent>()).Should().BeTrue();

            var notification = await context.Notifications.FirstOrDefaultAsync(n => n.RelatedApplicationId == applicationId);
            notification.Should().NotBeNull();
            notification!.UserId.Should().Be(userId);
            notification.Title.Should().Be("Application status updated");
            notification.Message.Should().Be("Status changed from Applied to Interview.");
        }
        finally
        {
            await harness.Stop();
        }
    }
}
