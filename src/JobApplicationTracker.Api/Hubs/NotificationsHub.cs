using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace JobApplicationTracker.Api.Hubs;

[Authorize]
public class NotificationsHub : Hub
{
}
