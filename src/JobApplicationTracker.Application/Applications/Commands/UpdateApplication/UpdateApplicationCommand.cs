using JobApplicationTracker.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTracker.Application.Applications.Commands.UpdateApplication;

public record UpdateApplicationCommand(
    Guid Id,
    string Company,
    string Role,
    DateTime AppliedDate) : IRequest;

public class UpdateApplicationCommandHandler : IRequestHandler<UpdateApplicationCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateApplicationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateApplicationCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Applications
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Application {request.Id} not found.");

        entity.Company = request.Company;
        entity.Role = request.Role;
        entity.AppliedDate = request.AppliedDate;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
