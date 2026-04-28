namespace JobApplicationTracker.Application.Reminders.Dtos;

public class ReminderDto
{
    public Guid Id { get; set; }
    public Guid ApplicationId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DueDateUtc { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
}
