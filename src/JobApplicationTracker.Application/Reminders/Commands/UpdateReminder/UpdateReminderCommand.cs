using FluentValidation;
using JobApplicationTracker.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTracker.Application.Reminders.Commands.UpdateReminder;

public record UpdateReminderCommand(
    Guid ApplicationId,
    Guid ReminderId,
    string Title,
    string? Description,
    DateTime DueDateUtc) : IRequest;

public class UpdateReminderCommandValidator : AbstractValidator<UpdateReminderCommand>
{
    public UpdateReminderCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(1000);
        RuleFor(x => x.DueDateUtc).NotEmpty();
    }
}

public class UpdateReminderCommandHandler : IRequestHandler<UpdateReminderCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateReminderCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateReminderCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Reminders
            .FirstOrDefaultAsync(r => r.Id == request.ReminderId && r.ApplicationId == request.ApplicationId, cancellationToken)
            ?? throw new KeyNotFoundException($"Reminder {request.ReminderId} not found.");

        entity.Title = request.Title;
        entity.Description = request.Description;
        entity.DueDateUtc = request.DueDateUtc;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
