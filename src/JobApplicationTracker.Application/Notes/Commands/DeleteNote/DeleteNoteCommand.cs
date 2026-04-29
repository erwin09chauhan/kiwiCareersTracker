using JobApplicationTracker.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTracker.Application.Notes.Commands.DeleteNote;

public record DeleteNoteCommand(Guid ApplicationId, Guid NoteId) : IRequest;

public class DeleteNoteCommandHandler : IRequestHandler<DeleteNoteCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteNoteCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteNoteCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Notes
            .FirstOrDefaultAsync(n => n.Id == request.NoteId && n.ApplicationId == request.ApplicationId, cancellationToken)
            ?? throw new KeyNotFoundException($"Note {request.NoteId} not found.");

        _context.Notes.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
