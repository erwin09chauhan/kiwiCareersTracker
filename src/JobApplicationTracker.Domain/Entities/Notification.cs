namespace JobApplicationTracker.Domain.Entities;

public class Notification : Common.BaseEntity
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime? ReadAtUtc { get; set; }
    public Guid? RelatedApplicationId { get; set; }
}
