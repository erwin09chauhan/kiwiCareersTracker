using FluentValidation;
using JobApplicationTracker.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTracker.Application.Contacts.Commands.UpdateContact;

public record UpdateContactCommand(
    Guid ApplicationId,
    Guid ContactId,
    string Name,
    string? Role,
    string? Email,
    string? Phone,
    string? LinkedInUrl) : IRequest;

public class UpdateContactCommandValidator : AbstractValidator<UpdateContactCommand>
{
    public UpdateContactCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Role).MaximumLength(100);
        RuleFor(x => x.Email).MaximumLength(255).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));
        RuleFor(x => x.Phone).MaximumLength(50);
        RuleFor(x => x.LinkedInUrl).MaximumLength(500);
    }
}

public class UpdateContactCommandHandler : IRequestHandler<UpdateContactCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateContactCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateContactCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Contacts
            .FirstOrDefaultAsync(c => c.Id == request.ContactId && c.ApplicationId == request.ApplicationId, cancellationToken)
            ?? throw new KeyNotFoundException($"Contact {request.ContactId} not found.");

        entity.Name = request.Name;
        entity.Role = request.Role;
        entity.Email = request.Email;
        entity.Phone = request.Phone;
        entity.LinkedInUrl = request.LinkedInUrl;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
