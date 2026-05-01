using JobApplicationTracker.Application.Applications.Commands.CreateApplication;
using JobApplicationTracker.Application.Applications.Commands.DeleteApplication;
using JobApplicationTracker.Application.Applications.Commands.UpdateApplication;
using JobApplicationTracker.Application.Applications.Commands.UpdateApplicationStatus;
using JobApplicationTracker.Application.Applications.Queries.GetApplicationById;
using JobApplicationTracker.Application.Applications.Queries.GetApplications;
using JobApplicationTracker.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationTracker.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/applications")]
public class ApplicationsController : ControllerBase
{
    private readonly ISender _sender;

    public ApplicationsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetApplications(
        [FromQuery] ApplicationStatus? status,
        [FromQuery] string? search,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortOrder,
        [FromQuery] string? cursor,
        [FromQuery] int limit = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(
            new GetApplicationsQuery(status, search, sortBy, sortOrder, cursor, limit),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetApplicationByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateApplicationCommand command, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateApplicationRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateApplicationCommand(id, request.Company, request.Role, request.AppliedDate), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _sender.Send(new DeleteApplicationCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateApplicationStatusRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateApplicationStatusCommand(id, request.NewStatus, request.Notes), cancellationToken);
        return NoContent();
    }

    public record UpdateApplicationRequest(string Company, string Role, DateTime AppliedDate);
    public record UpdateApplicationStatusRequest(ApplicationStatus NewStatus, string? Notes);
}
