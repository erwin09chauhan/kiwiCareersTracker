using FluentValidation;
using JobApplicationTracker.Application.Common.Interfaces;
using JobApplicationTracker.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTracker.Application.Reminders.Commands.CreateReminder;

public record CreateReminderCommand(
    Guid ApplicationId,
    string Title,
    string? Description,
    DateTime DueDateUtc) : IRequest<Guid>;

public class CreateReminderCommandValidator : AbstractValidator<CreateReminderCommand>
{
    public CreateReminderCommandValidator()
    {
        RuleFor(x => x.ApplicationId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(1000);
        RuleFor(x => x.DueDateUtc).NotEmpty();
    }
}

public class CreateReminderCommandHandler : IRequestHandler<CreateReminderCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateReminderCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateReminderCommand request, CancellationToken cancellationToken)
    {
        var applicationExists = await _context.Applications
            .AnyAsync(a => a.Id == request.ApplicationId, cancellationToken);

        if (!applicationExists)
        {
            throw new KeyNotFoundException($"Application {request.ApplicationId} not found.");
        }

        var entity = new Reminder
        {
            ApplicationId = request.ApplicationId,
            Title = request.Title,
            Description = request.Description,
            DueDateUtc = request.DueDateUtc
        };

        _context.Reminders.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
