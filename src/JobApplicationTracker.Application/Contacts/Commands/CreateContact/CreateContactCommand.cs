using FluentValidation;
using JobApplicationTracker.Application.Common.Interfaces;
using JobApplicationTracker.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTracker.Application.Contacts.Commands.CreateContact;

public record CreateContactCommand(
    Guid ApplicationId,
    string Name,
    string? Role,
    string? Email,
    string? Phone,
    string? LinkedInUrl) : IRequest<Guid>;

public class CreateContactCommandValidator : AbstractValidator<CreateContactCommand>
{
    public CreateContactCommandValidator()
    {
        RuleFor(x => x.ApplicationId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Role).MaximumLength(100);
        RuleFor(x => x.Email).MaximumLength(255).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));
        RuleFor(x => x.Phone).MaximumLength(50);
        RuleFor(x => x.LinkedInUrl).MaximumLength(500);
    }
}

public class CreateContactCommandHandler : IRequestHandler<CreateContactCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateContactCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateContactCommand request, CancellationToken cancellationToken)
    {
        var applicationExists = await _context.Applications
            .AnyAsync(a => a.Id == request.ApplicationId, cancellationToken);

        if (!applicationExists)
        {
            throw new KeyNotFoundException($"Application {request.ApplicationId} not found.");
        }

        var entity = new Contact
        {
            ApplicationId = request.ApplicationId,
            Name = request.Name,
            Role = request.Role,
            Email = request.Email,
            Phone = request.Phone,
            LinkedInUrl = request.LinkedInUrl
        };

        _context.Contacts.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
