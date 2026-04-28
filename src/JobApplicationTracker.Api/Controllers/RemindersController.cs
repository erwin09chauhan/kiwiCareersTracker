using JobApplicationTracker.Application.Reminders.Commands.CompleteReminder;
using JobApplicationTracker.Application.Reminders.Commands.CreateReminder;
using JobApplicationTracker.Application.Reminders.Commands.DeleteReminder;
using JobApplicationTracker.Application.Reminders.Commands.UpdateReminder;
using JobApplicationTracker.Application.Reminders.Queries.GetReminders;
using JobApplicationTracker.Application.Reminders.Queries.GetUpcomingReminders;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationTracker.Api.Controllers;

[ApiController]
[Route("api/v1")]
public class RemindersController : ControllerBase
{
    private readonly ISender _sender;

    public RemindersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("applications/{applicationId:guid}/reminders")]
    public async Task<IActionResult> GetReminders(Guid applicationId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetRemindersQuery(applicationId), cancellationToken);
        return Ok(result);
    }

    [HttpPost("applications/{applicationId:guid}/reminders")]
    public async Task<IActionResult> Create(Guid applicationId, [FromBody] CreateReminderRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(new CreateReminderCommand(applicationId, request.Title, request.Description, request.DueDateUtc), cancellationToken);
        return CreatedAtAction(nameof(GetReminders), new { applicationId }, new { id });
    }

    [HttpPut("applications/{applicationId:guid}/reminders/{reminderId:guid}")]
    public async Task<IActionResult> Update(Guid applicationId, Guid reminderId, [FromBody] UpdateReminderRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateReminderCommand(applicationId, reminderId, request.Title, request.Description, request.DueDateUtc), cancellationToken);
        return NoContent();
    }

    [HttpDelete("applications/{applicationId:guid}/reminders/{reminderId:guid}")]
    public async Task<IActionResult> Delete(Guid applicationId, Guid reminderId, CancellationToken cancellationToken)
    {
        await _sender.Send(new DeleteReminderCommand(applicationId, reminderId), cancellationToken);
        return NoContent();
    }

    [HttpPatch("applications/{applicationId:guid}/reminders/{reminderId:guid}/complete")]
    public async Task<IActionResult> Complete(Guid applicationId, Guid reminderId, CancellationToken cancellationToken)
    {
        await _sender.Send(new CompleteReminderCommand(applicationId, reminderId), cancellationToken);
        return NoContent();
    }

    [HttpGet("reminders/upcoming")]
    public async Task<IActionResult> GetUpcoming(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetUpcomingRemindersQuery(), cancellationToken);
        return Ok(result);
    }

    public record CreateReminderRequest(string Title, string? Description, DateTime DueDateUtc);
    public record UpdateReminderRequest(string Title, string? Description, DateTime DueDateUtc);
}
