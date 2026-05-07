using JobApplicationTracker.Application.Applications.Commands.CreateApplication;
using JobApplicationTracker.Application.Common.Interfaces;
using JobApplicationTracker.UnitTests.Common;
using FluentAssertions;
using NSubstitute;

namespace JobApplicationTracker.UnitTests.Applications;

public class CreateApplicationCommandTests
{
    [Fact]
    public async Task Handle_ShouldCreateApplication_WithCurrentUserId()
    {
        await using var context = TestDbContextFactory.Create();

        var userId = Guid.NewGuid();
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.UserId.Returns(userId);

        var handler = new CreateApplicationCommandHandler(context, currentUser);

        var command = new CreateApplicationCommand("Acme", "Backend Engineer", DateTime.UtcNow);

        var id = await handler.Handle(command, CancellationToken.None);

        var entity = await context.Applications.FindAsync(id);

        entity.Should().NotBeNull();
        entity!.UserId.Should().Be(userId);
        entity.Company.Should().Be("Acme");
        entity.Role.Should().Be("Backend Engineer");
        entity.Status.Should().Be(JobApplicationTracker.Domain.Enums.ApplicationStatus.Applied);
    }
}
