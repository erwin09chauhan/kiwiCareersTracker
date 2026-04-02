namespace JobApplicationTracker.Domain.Entities;

public class Note : Common.BaseEntity
{
    public Guid ApplicationId { get; set; }
    public JobApplication? Application { get; set; }

    public string Content { get; set; } = string.Empty;
}
