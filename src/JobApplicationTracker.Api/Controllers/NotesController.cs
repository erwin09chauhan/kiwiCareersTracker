using JobApplicationTracker.Application.Notes.Commands.CreateNote;
using JobApplicationTracker.Application.Notes.Commands.DeleteNote;
using JobApplicationTracker.Application.Notes.Commands.UpdateNote;
using JobApplicationTracker.Application.Notes.Queries.GetNotes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationTracker.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/applications/{applicationId:guid}/notes")]
public class NotesController : ControllerBase
{
    private readonly ISender _sender;

    public NotesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(Guid applicationId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetNotesQuery(applicationId), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid applicationId, [FromBody] NoteRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(new CreateNoteCommand(applicationId, request.Content), cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { applicationId }, new { id });
    }

    [HttpPut("{noteId:guid}")]
    public async Task<IActionResult> Update(Guid applicationId, Guid noteId, [FromBody] NoteRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateNoteCommand(applicationId, noteId, request.Content), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{noteId:guid}")]
    public async Task<IActionResult> Delete(Guid applicationId, Guid noteId, CancellationToken cancellationToken)
    {
        await _sender.Send(new DeleteNoteCommand(applicationId, noteId), cancellationToken);
        return NoContent();
    }

    public record NoteRequest(string Content);
}
