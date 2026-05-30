using JobApplicationTracker.Application.Applications.Commands.UpdateApplicationStatus;
using JobApplicationTracker.Application.Common.Interfaces;
using JobApplicationTracker.Domain.Entities;
using JobApplicationTracker.Domain.Enums;
using JobApplicationTracker.Domain.Events;
using JobApplicationTracker.UnitTests.Common;
using FluentAssertions;
using NSubstitute;

namespace JobApplicationTracker.UnitTests.Applications;

public class UpdateApplicationStatusCommandTests
{
    [Fact]
    public async Task Handle_ShouldUpdateStatus_AndPublishStatusChangedEvent()
    {
        await using var context = TestDbContextFactory.Create();

        var userId = Guid.NewGuid();
        var application = new JobApplication
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Company = "Acme",
            Role = "Backend Engineer",
            Status = ApplicationStatus.Applied,
            AppliedDate = DateTime.UtcNow
        };

        context.Applications.Add(application);
        await context.SaveChangesAsync();

        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.UserId.Returns(userId);

        var publishEndpoint = Substitute.For<IPublishEndpointService>();
        var cache = Substitute.For<ICacheService>();

        var handler = new UpdateApplicationStatusCommandHandler(context, currentUser, publishEndpoint, cache);

        var command = new UpdateApplicationStatusCommand(application.Id, ApplicationStatus.Interview, "Moved to interview");

        await handler.Handle(command, CancellationToken.None);

        var updated = await context.Applications.FindAsync(application.Id);
        updated!.Status.Should().Be(ApplicationStatus.Interview);

        await publishEndpoint.Received(1).PublishAsync(
            Arg.Is<ApplicationStatusChangedEvent>(e =>
                e.ApplicationId == application.Id &&
                e.UserId == userId &&
                e.FromStatus == ApplicationStatus.Applied &&
                e.ToStatus == ApplicationStatus.Interview),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenApplicationNotFound()
    {
        await using var context = TestDbContextFactory.Create();

        var currentUser = Substitute.For<ICurrentUserService>();
        var publishEndpoint = Substitute.For<IPublishEndpointService>();
        var cache = Substitute.For<ICacheService>();

        var handler = new UpdateApplicationStatusCommandHandler(context, currentUser, publishEndpoint, cache);

        var command = new UpdateApplicationStatusCommand(Guid.NewGuid(), ApplicationStatus.Interview, null);

        var act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
