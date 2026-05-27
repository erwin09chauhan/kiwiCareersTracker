using JobApplicationTracker.Application.Common.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace JobApplicationTracker.Api.Hubs;

public class SignalRNotificationPusher : INotificationPusher
{
    private readonly IHubContext<NotificationsHub> _hubContext;

    public SignalRNotificationPusher(IHubContext<NotificationsHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task PushNotificationAsync(Guid userId, string title, string message, Guid? relatedApplicationId, CancellationToken cancellationToken = default)
    {
        return _hubContext.Clients.User(userId.ToString()).SendAsync("notification", new
        {
            title,
            message,
            relatedApplicationId
        }, cancellationToken);
    }
}
