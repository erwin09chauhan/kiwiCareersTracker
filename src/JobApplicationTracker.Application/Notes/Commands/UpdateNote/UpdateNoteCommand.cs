using FluentValidation;
using JobApplicationTracker.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTracker.Application.Notes.Commands.UpdateNote;

public record UpdateNoteCommand(Guid ApplicationId, Guid NoteId, string Content) : IRequest;

public class UpdateNoteCommandValidator : AbstractValidator<UpdateNoteCommand>
{
    public UpdateNoteCommandValidator()
    {
        RuleFor(x => x.Content).NotEmpty().MaximumLength(4000);
    }
}

public class UpdateNoteCommandHandler : IRequestHandler<UpdateNoteCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateNoteCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Notes
            .FirstOrDefaultAsync(n => n.Id == request.NoteId && n.ApplicationId == request.ApplicationId, cancellationToken)
            ?? throw new KeyNotFoundException($"Note {request.NoteId} not found.");

        entity.Content = request.Content;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
