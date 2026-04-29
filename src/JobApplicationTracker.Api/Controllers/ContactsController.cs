using JobApplicationTracker.Application.Contacts.Commands.CreateContact;
using JobApplicationTracker.Application.Contacts.Commands.DeleteContact;
using JobApplicationTracker.Application.Contacts.Commands.UpdateContact;
using JobApplicationTracker.Application.Contacts.Queries.GetContactById;
using JobApplicationTracker.Application.Contacts.Queries.GetContacts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationTracker.Api.Controllers;

[ApiController]
[Route("api/v1/applications/{applicationId:guid}/contacts")]
public class ContactsController : ControllerBase
{
    private readonly ISender _sender;

    public ContactsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(Guid applicationId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetContactsQuery(applicationId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{contactId:guid}")]
    public async Task<IActionResult> GetById(Guid applicationId, Guid contactId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetContactByIdQuery(applicationId, contactId), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid applicationId, [FromBody] ContactRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(new CreateContactCommand(applicationId, request.Name, request.Role, request.Email, request.Phone, request.LinkedInUrl), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { applicationId, contactId = id }, new { id });
    }

    [HttpPut("{contactId:guid}")]
    public async Task<IActionResult> Update(Guid applicationId, Guid contactId, [FromBody] ContactRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateContactCommand(applicationId, contactId, request.Name, request.Role, request.Email, request.Phone, request.LinkedInUrl), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{contactId:guid}")]
    public async Task<IActionResult> Delete(Guid applicationId, Guid contactId, CancellationToken cancellationToken)
    {
        await _sender.Send(new DeleteContactCommand(applicationId, contactId), cancellationToken);
        return NoContent();
    }

    public record ContactRequest(string Name, string? Role, string? Email, string? Phone, string? LinkedInUrl);
}
