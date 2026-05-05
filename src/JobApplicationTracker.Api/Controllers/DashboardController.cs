using JobApplicationTracker.Application.Dashboard.Queries.GetDashboardSummary;
using JobApplicationTracker.Application.Dashboard.Queries.GetRecentActivity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationTracker.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly ISender _sender;

    public DashboardController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetDashboardSummaryQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("activity")]
    public async Task<IActionResult> GetActivity([FromQuery] int limit = 20, CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(new GetRecentActivityQuery(limit), cancellationToken);
        return Ok(result);
    }
}
