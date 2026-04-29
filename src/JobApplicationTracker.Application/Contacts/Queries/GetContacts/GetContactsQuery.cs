using AutoMapper;
using JobApplicationTracker.Application.Common.Interfaces;
using JobApplicationTracker.Application.Contacts.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTracker.Application.Contacts.Queries.GetContacts;

public record GetContactsQuery(Guid ApplicationId) : IRequest<List<ContactDto>>;

public class GetContactsQueryHandler : IRequestHandler<GetContactsQuery, List<ContactDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetContactsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ContactDto>> Handle(GetContactsQuery request, CancellationToken cancellationToken)
    {
        var contacts = await _context.Contacts
            .AsNoTracking()
            .Where(c => c.ApplicationId == request.ApplicationId)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<ContactDto>>(contacts);
    }
}
