using AutoMapper;
using JobApplicationTracker.Application.Common.Interfaces;
using JobApplicationTracker.Application.Reminders.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTracker.Application.Reminders.Queries.GetUpcomingReminders;

public record GetUpcomingRemindersQuery : IRequest<List<ReminderDto>>;

public class GetUpcomingRemindersQueryHandler : IRequestHandler<GetUpcomingRemindersQuery, List<ReminderDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public GetUpcomingRemindersQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser, IMapper mapper)
    {
        _context = context;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<List<ReminderDto>> Handle(GetUpcomingRemindersQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? Guid.Empty;

        var reminders = await _context.Reminders
            .AsNoTracking()
            .Where(r => !r.IsCompleted && r.Application!.UserId == userId)
            .OrderBy(r => r.DueDateUtc)
            .Take(50)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<ReminderDto>>(reminders);
    }
}
