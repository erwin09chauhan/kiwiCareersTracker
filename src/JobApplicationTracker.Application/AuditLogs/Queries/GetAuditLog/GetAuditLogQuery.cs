using System.Text;
using AutoMapper;
using JobApplicationTracker.Application.AuditLogs.Dtos;
using JobApplicationTracker.Application.Common.Interfaces;
using JobApplicationTracker.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTracker.Application.AuditLogs.Queries.GetAuditLog;

public record GetAuditLogQuery(string? Cursor, int Limit = 20) : IRequest<PaginatedList<AuditLogEntryDto>>;

public class GetAuditLogQueryHandler : IRequestHandler<GetAuditLogQuery, PaginatedList<AuditLogEntryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAuditLogQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<AuditLogEntryDto>> Handle(GetAuditLogQuery request, CancellationToken cancellationToken)
    {
        var query = _context.AuditLogs.AsNoTracking().OrderByDescending(l => l.CreatedAtUtc).ThenByDescending(l => l.Id).AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Cursor))
        {
            var (cursorCreatedAt, cursorId) = DecodeCursor(request.Cursor);
            query = query.Where(l =>
                l.CreatedAtUtc < cursorCreatedAt ||
                (l.CreatedAtUtc == cursorCreatedAt && l.Id.CompareTo(cursorId) < 0));
        }

        var limit = request.Limit <= 0 ? 20 : Math.Min(request.Limit, 100);

        var items = await query.Take(limit + 1).ToListAsync(cancellationToken);

        var hasMore = items.Count > limit;
        if (hasMore)
        {
            items = items.Take(limit).ToList();
        }

        var dtos = _mapper.Map<List<AuditLogEntryDto>>(items);

        return new PaginatedList<AuditLogEntryDto>
        {
            Items = dtos,
            HasMore = hasMore,
            NextCursor = hasMore ? EncodeCursor(items[^1].CreatedAtUtc, items[^1].Id) : null
        };
    }

    private static string EncodeCursor(DateTime createdAtUtc, Guid id) =>
        Convert.ToBase64String(Encoding.UTF8.GetBytes($"{createdAtUtc:O}|{id}"));

    private static (DateTime, Guid) DecodeCursor(string cursor)
    {
        var raw = Encoding.UTF8.GetString(Convert.FromBase64String(cursor));
        var parts = raw.Split('|');
        return (DateTime.Parse(parts[0]).ToUniversalTime(), Guid.Parse(parts[1]));
    }
}
