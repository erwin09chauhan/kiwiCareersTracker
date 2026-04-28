using JobApplicationTracker.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTracker.Application.Reminders.Commands.DeleteReminder;

public record DeleteReminderCommand(Guid ApplicationId, Guid ReminderId) : IRequest;

public class DeleteReminderCommandHandler : IRequestHandler<DeleteReminderCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteReminderCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteReminderCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Reminders
            .FirstOrDefaultAsync(r => r.Id == request.ReminderId && r.ApplicationId == request.ApplicationId, cancellationToken)
            ?? throw new KeyNotFoundException($"Reminder {request.ReminderId} not found.");

        _context.Reminders.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
