using JobApplicationTracker.Application.AuditLogs.Queries.GetApplicationAuditLog;
using JobApplicationTracker.Application.AuditLogs.Queries.GetAuditLog;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationTracker.Api.Controllers;

[ApiController]
[Route("api/v1")]
public class AuditLogController : ControllerBase
{
    private readonly ISender _sender;

    public AuditLogController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("applications/{applicationId:guid}/audit")]
    public async Task<IActionResult> GetForApplication(Guid applicationId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetApplicationAuditLogQuery(applicationId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("audit")]
    public async Task<IActionResult> GetAll([FromQuery] string? cursor, [FromQuery] int limit = 20, CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(new GetAuditLogQuery(cursor, limit), cancellationToken);
        return Ok(result);
    }
}
