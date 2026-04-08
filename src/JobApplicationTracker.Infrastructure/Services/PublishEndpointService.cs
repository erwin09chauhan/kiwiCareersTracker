using JobApplicationTracker.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace JobApplicationTracker.Infrastructure.Services;

public class PublishEndpointService : IPublishEndpointService
{
    private readonly ILogger<PublishEndpointService> _logger;

    public PublishEndpointService(ILogger<PublishEndpointService> logger)
    {
        _logger = logger;
    }

    public Task PublishAsync<T>(T message, CancellationToken cancellationToken) where T : class
    {
        _logger.LogInformation("Publishing event {EventType}: {@Event}", typeof(T).Name, message);
        return Task.CompletedTask;
    }
}
