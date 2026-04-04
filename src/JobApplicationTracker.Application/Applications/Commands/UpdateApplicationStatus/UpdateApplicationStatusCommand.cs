using JobApplicationTracker.Application.Common.Interfaces;
using JobApplicationTracker.Domain.Enums;
using JobApplicationTracker.Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTracker.Application.Applications.Commands.UpdateApplicationStatus;

public record UpdateApplicationStatusCommand(
    Guid Id,
    ApplicationStatus NewStatus,
    string? Notes) : IRequest;

public class UpdateApplicationStatusCommandHandler : IRequestHandler<UpdateApplicationStatusCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IPublishEndpointService _publishEndpoint;

    public UpdateApplicationStatusCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IPublishEndpointService publishEndpoint)
    {
        _context = context;
        _currentUser = currentUser;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Handle(UpdateApplicationStatusCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Applications
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Application {request.Id} not found.");

        var oldStatus = entity.Status;
        entity.Status = request.NewStatus;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        await _publishEndpoint.PublishAsync(new ApplicationStatusChangedEvent(
            entity.Id,
            entity.UserId,
            oldStatus,
            entity.Status,
            DateTime.UtcNow), cancellationToken);
    }
}
