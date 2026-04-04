using JobApplicationTracker.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTracker.Application.Applications.Commands.DeleteApplication;

public record DeleteApplicationCommand(Guid Id) : IRequest;

public class DeleteApplicationCommandHandler : IRequestHandler<DeleteApplicationCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteApplicationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteApplicationCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Applications
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Application {request.Id} not found.");

        _context.Applications.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
