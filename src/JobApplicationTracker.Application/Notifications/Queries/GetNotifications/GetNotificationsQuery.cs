using System.Text;
using AutoMapper;
using JobApplicationTracker.Application.Common.Interfaces;
using JobApplicationTracker.Application.Common.Models;
using JobApplicationTracker.Application.Notifications.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTracker.Application.Notifications.Queries.GetNotifications;

public record GetNotificationsQuery(string? Cursor, int Limit = 20) : IRequest<PaginatedList<NotificationDto>>;

public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, PaginatedList<NotificationDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public GetNotificationsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser, IMapper mapper)
    {
        _context = context;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<PaginatedList<NotificationDto>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? Guid.Empty;

        var query = _context.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAtUtc)
            .ThenByDescending(n => n.Id)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Cursor))
        {
            var (cursorCreatedAt, cursorId) = DecodeCursor(request.Cursor);
            query = query.Where(n =>
                n.CreatedAtUtc < cursorCreatedAt ||
                (n.CreatedAtUtc == cursorCreatedAt && n.Id.CompareTo(cursorId) < 0));
        }

        var limit = request.Limit <= 0 ? 20 : Math.Min(request.Limit, 100);

        var items = await query.Take(limit + 1).ToListAsync(cancellationToken);

        var hasMore = items.Count > limit;
        if (hasMore)
        {
            items = items.Take(limit).ToList();
        }

        var dtos = _mapper.Map<List<NotificationDto>>(items);

        return new PaginatedList<NotificationDto>
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
