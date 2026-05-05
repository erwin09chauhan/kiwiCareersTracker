using JobApplicationTracker.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTracker.Application.Dashboard.Queries.GetRecentActivity;

public record GetRecentActivityQuery(int Limit = 20) : IRequest<List<ActivityItemDto>>;

public class ActivityItemDto
{
    public string Type { get; set; } = string.Empty;
    public Guid? ApplicationId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Detail { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

public class GetRecentActivityQueryHandler : IRequestHandler<GetRecentActivityQuery, List<ActivityItemDto>>
{
    private readonly IApplicationDbContext _context;

    public GetRecentActivityQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ActivityItemDto>> Handle(GetRecentActivityQuery request, CancellationToken cancellationToken)
    {
        var limit = request.Limit <= 0 ? 20 : Math.Min(request.Limit, 100);

        var auditItems = await _context.AuditLogs
            .AsNoTracking()
            .OrderByDescending(l => l.CreatedAtUtc)
            .Take(limit)
            .Select(l => new ActivityItemDto
            {
                Type = "StatusChange",
                ApplicationId = l.ApplicationId,
                Title = l.FromStatus.HasValue
                    ? $"Status changed from {l.FromStatus} to {l.ToStatus}"
                    : $"Status set to {l.ToStatus}",
                Detail = l.Notes,
                CreatedAtUtc = l.CreatedAtUtc
            })
            .ToListAsync(cancellationToken);

        var notificationItems = await _context.Notifications
            .AsNoTracking()
            .OrderByDescending(n => n.CreatedAtUtc)
            .Take(limit)
            .Select(n => new ActivityItemDto
            {
                Type = "Notification",
                ApplicationId = n.RelatedApplicationId,
                Title = n.Title,
                Detail = n.Message,
                CreatedAtUtc = n.CreatedAtUtc
            })
            .ToListAsync(cancellationToken);

        return auditItems
            .Concat(notificationItems)
            .OrderByDescending(a => a.CreatedAtUtc)
            .Take(limit)
            .ToList();
    }
}
