using System.Text;
using AutoMapper;
using JobApplicationTracker.Application.Applications.Dtos;
using JobApplicationTracker.Application.Common.Interfaces;
using JobApplicationTracker.Application.Common.Models;
using JobApplicationTracker.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTracker.Application.Applications.Queries.GetApplications;

public record GetApplicationsQuery(
    ApplicationStatus? Status,
    string? Search,
    string? SortBy,
    string? SortOrder,
    string? Cursor,
    int Limit = 20) : IRequest<PaginatedList<JobApplicationDto>>;

public class GetApplicationsQueryHandler : IRequestHandler<GetApplicationsQuery, PaginatedList<JobApplicationDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetApplicationsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<JobApplicationDto>> Handle(GetApplicationsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Applications.AsNoTracking().AsQueryable();

        if (request.Status.HasValue)
        {
            query = query.Where(a => a.Status == request.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(a =>
                a.Company.ToLower().Contains(search.ToLower()) ||
                a.Role.ToLower().Contains(search.ToLower()));
        }

        var sortOrder = string.Equals(request.SortOrder, "desc", StringComparison.OrdinalIgnoreCase) ? "desc" : "asc";

        query = request.SortBy?.ToLowerInvariant() switch
        {
            "company" => sortOrder == "desc" ? query.OrderByDescending(a => a.Company).ThenByDescending(a => a.Id)
                                               : query.OrderBy(a => a.Company).ThenBy(a => a.Id),
            "status" => sortOrder == "desc" ? query.OrderByDescending(a => a.Status).ThenByDescending(a => a.Id)
                                             : query.OrderBy(a => a.Status).ThenBy(a => a.Id),
            _ => sortOrder == "desc" ? query.OrderByDescending(a => a.AppliedDate).ThenByDescending(a => a.Id)
                                      : query.OrderBy(a => a.AppliedDate).ThenBy(a => a.Id)
        };

        if (!string.IsNullOrWhiteSpace(request.Cursor))
        {
            var lastId = DecodeCursor(request.Cursor);
            query = query.Where(a => a.Id.CompareTo(lastId) > 0);
        }

        var limit = request.Limit <= 0 ? 20 : Math.Min(request.Limit, 100);

        var items = await query.Take(limit + 1).ToListAsync(cancellationToken);

        var hasMore = items.Count > limit;
        if (hasMore)
        {
            items = items.Take(limit).ToList();
        }

        var dtos = _mapper.Map<List<JobApplicationDto>>(items);

        return new PaginatedList<JobApplicationDto>
        {
            Items = dtos,
            HasMore = hasMore,
            NextCursor = hasMore ? EncodeCursor(items[^1].Id) : null
        };
    }

    private static string EncodeCursor(Guid id) =>
        Convert.ToBase64String(Encoding.UTF8.GetBytes(id.ToString()));

    private static Guid DecodeCursor(string cursor) =>
        Guid.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(cursor)));
}
