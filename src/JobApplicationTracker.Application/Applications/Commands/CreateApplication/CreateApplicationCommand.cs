using JobApplicationTracker.Domain.Entities;
using JobApplicationTracker.Domain.Enums;
using JobApplicationTracker.Application.Common.Interfaces;
using MediatR;

namespace JobApplicationTracker.Application.Applications.Commands.CreateApplication;

public record CreateApplicationCommand(
    string Company,
    string Role,
    DateTime AppliedDate) : IRequest<Guid>;

public class CreateApplicationCommandHandler : IRequestHandler<CreateApplicationCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateApplicationCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(CreateApplicationCommand request, CancellationToken cancellationToken)
    {
        var entity = new JobApplication
        {
            UserId = _currentUser.UserId ?? Guid.Empty,
            Company = request.Company,
            Role = request.Role,
            AppliedDate = request.AppliedDate,
            Status = ApplicationStatus.Applied
        };

        _context.Applications.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
