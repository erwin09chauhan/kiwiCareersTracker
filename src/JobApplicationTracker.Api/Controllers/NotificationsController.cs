using JobApplicationTracker.Application.Notifications.Commands.DeleteNotification;
using JobApplicationTracker.Application.Notifications.Commands.MarkAllNotificationsRead;
using JobApplicationTracker.Application.Notifications.Commands.MarkNotificationRead;
using JobApplicationTracker.Application.Notifications.Queries.GetNotifications;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationTracker.Api.Controllers;

[ApiController]
[Route("api/v1/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly ISender _sender;

    public NotificationsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? cursor, [FromQuery] int limit = 20, CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(new GetNotificationsQuery(cursor, limit), cancellationToken);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid id, CancellationToken cancellationToken)
    {
        await _sender.Send(new MarkNotificationReadCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPatch("read-all")]
    public async Task<IActionResult> MarkAllRead(CancellationToken cancellationToken)
    {
        await _sender.Send(new MarkAllNotificationsReadCommand(), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _sender.Send(new DeleteNotificationCommand(id), cancellationToken);
        return NoContent();
    }
}
