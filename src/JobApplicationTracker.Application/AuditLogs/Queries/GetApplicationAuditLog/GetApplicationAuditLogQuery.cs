using AutoMapper;
using JobApplicationTracker.Application.AuditLogs.Dtos;
using JobApplicationTracker.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTracker.Application.AuditLogs.Queries.GetApplicationAuditLog;

public record GetApplicationAuditLogQuery(Guid ApplicationId) : IRequest<List<AuditLogEntryDto>>;

public class GetApplicationAuditLogQueryHandler : IRequestHandler<GetApplicationAuditLogQuery, List<AuditLogEntryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetApplicationAuditLogQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<AuditLogEntryDto>> Handle(GetApplicationAuditLogQuery request, CancellationToken cancellationToken)
    {
        var entries = await _context.AuditLogs
            .AsNoTracking()
            .Where(l => l.ApplicationId == request.ApplicationId)
            .OrderByDescending(l => l.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<AuditLogEntryDto>>(entries);
    }
}
