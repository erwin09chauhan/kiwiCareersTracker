using AutoMapper;
using JobApplicationTracker.Application.Common.Interfaces;
using JobApplicationTracker.Application.Reminders.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTracker.Application.Reminders.Queries.GetReminders;

public record GetRemindersQuery(Guid ApplicationId) : IRequest<List<ReminderDto>>;

public class GetRemindersQueryHandler : IRequestHandler<GetRemindersQuery, List<ReminderDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetRemindersQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ReminderDto>> Handle(GetRemindersQuery request, CancellationToken cancellationToken)
    {
        var reminders = await _context.Reminders
            .AsNoTracking()
            .Where(r => r.ApplicationId == request.ApplicationId)
            .OrderBy(r => r.DueDateUtc)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<ReminderDto>>(reminders);
    }
}
