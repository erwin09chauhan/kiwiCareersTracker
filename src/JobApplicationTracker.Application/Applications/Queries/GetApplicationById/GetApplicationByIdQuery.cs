using AutoMapper;
using JobApplicationTracker.Application.Applications.Dtos;
using JobApplicationTracker.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTracker.Application.Applications.Queries.GetApplicationById;

public record GetApplicationByIdQuery(Guid Id) : IRequest<JobApplicationDto>;

public class GetApplicationByIdQueryHandler : IRequestHandler<GetApplicationByIdQuery, JobApplicationDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetApplicationByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<JobApplicationDto> Handle(GetApplicationByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Applications
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Application {request.Id} not found.");

        return _mapper.Map<JobApplicationDto>(entity);
    }
}
