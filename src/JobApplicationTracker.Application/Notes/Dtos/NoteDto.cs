namespace JobApplicationTracker.Application.Notes.Dtos;

public class NoteDto
{
    public Guid Id { get; set; }
    public Guid ApplicationId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}
