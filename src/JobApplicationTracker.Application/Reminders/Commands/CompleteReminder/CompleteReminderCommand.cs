using JobApplicationTracker.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTracker.Application.Reminders.Commands.CompleteReminder;

public record CompleteReminderCommand(Guid ApplicationId, Guid ReminderId) : IRequest;

public class CompleteReminderCommandHandler : IRequestHandler<CompleteReminderCommand>
{
    private readonly IApplicationDbContext _context;

    public CompleteReminderCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(CompleteReminderCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Reminders
            .FirstOrDefaultAsync(r => r.Id == request.ReminderId && r.ApplicationId == request.ApplicationId, cancellationToken)
            ?? throw new KeyNotFoundException($"Reminder {request.ReminderId} not found.");

        entity.IsCompleted = true;
        entity.CompletedAtUtc = DateTime.UtcNow;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
