namespace JobApplicationTracker.Application.Notifications.Dtos;

public class NotificationDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime? ReadAtUtc { get; set; }
    public Guid? RelatedApplicationId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
