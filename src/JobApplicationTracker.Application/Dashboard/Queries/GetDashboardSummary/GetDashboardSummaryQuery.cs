using JobApplicationTracker.Application.Common.Interfaces;
using JobApplicationTracker.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTracker.Application.Dashboard.Queries.GetDashboardSummary;

public record GetDashboardSummaryQuery : IRequest<DashboardSummaryDto>;

public class DashboardSummaryDto
{
    public Dictionary<string, int> CountsByStatus { get; set; } = new();
    public int Total { get; set; }
}

public class GetDashboardSummaryQueryHandler : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
{
    private const string CacheKey = "dashboard:summary";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    private readonly IApplicationDbContext _context;
    private readonly ICacheService _cache;
    private readonly ICurrentUserService _currentUser;

    public GetDashboardSummaryQueryHandler(IApplicationDbContext context, ICacheService cache, ICurrentUserService currentUser)
    {
        _context = context;
        _cache = cache;
        _currentUser = currentUser;
    }

    public async Task<DashboardSummaryDto> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? Guid.Empty;
        var cacheKey = $"{CacheKey}:{userId}";

        var cached = await _cache.GetAsync<DashboardSummaryDto>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            return cached;
        }

        var counts = await _context.Applications
            .AsNoTracking()
            .Where(a => a.UserId == userId)
            .GroupBy(a => a.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var dto = new DashboardSummaryDto
        {
            CountsByStatus = Enum.GetValues<ApplicationStatus>()
                .ToDictionary(s => s.ToString(), s => counts.FirstOrDefault(c => c.Status == s)?.Count ?? 0),
            Total = counts.Sum(c => c.Count)
        };

        await _cache.SetAsync(cacheKey, dto, CacheDuration, cancellationToken);

        return dto;
    }
}
