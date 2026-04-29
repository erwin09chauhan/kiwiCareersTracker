using JobApplicationTracker.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTracker.Application.Contacts.Commands.DeleteContact;

public record DeleteContactCommand(Guid ApplicationId, Guid ContactId) : IRequest;

public class DeleteContactCommandHandler : IRequestHandler<DeleteContactCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteContactCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteContactCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Contacts
            .FirstOrDefaultAsync(c => c.Id == request.ContactId && c.ApplicationId == request.ApplicationId, cancellationToken)
            ?? throw new KeyNotFoundException($"Contact {request.ContactId} not found.");

        _context.Contacts.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
