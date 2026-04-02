namespace JobApplicationTracker.Domain.Entities;

public class Reminder : Common.BaseEntity
{
    public Guid ApplicationId { get; set; }
    public JobApplication? Application { get; set; }

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DueDateUtc { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
}
