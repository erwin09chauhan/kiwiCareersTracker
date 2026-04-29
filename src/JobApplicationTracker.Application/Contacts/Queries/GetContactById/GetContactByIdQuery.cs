using AutoMapper;
using JobApplicationTracker.Application.Common.Interfaces;
using JobApplicationTracker.Application.Contacts.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTracker.Application.Contacts.Queries.GetContactById;

public record GetContactByIdQuery(Guid ApplicationId, Guid ContactId) : IRequest<ContactDto>;

public class GetContactByIdQueryHandler : IRequestHandler<GetContactByIdQuery, ContactDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetContactByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ContactDto> Handle(GetContactByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Contacts
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.ContactId && c.ApplicationId == request.ApplicationId, cancellationToken)
            ?? throw new KeyNotFoundException($"Contact {request.ContactId} not found.");

        return _mapper.Map<ContactDto>(entity);
    }
}
