namespace JobApplicationTracker.Application.Common.Interfaces;

public interface INotificationPusher
{
    Task PushNotificationAsync(Guid userId, string title, string message, Guid? relatedApplicationId, CancellationToken cancellationToken = default);
}
