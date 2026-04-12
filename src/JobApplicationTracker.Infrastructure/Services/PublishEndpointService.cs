using JobApplicationTracker.Application.Common.Interfaces;
using MassTransit;

namespace JobApplicationTracker.Infrastructure.Services;

public class PublishEndpointService : IPublishEndpointService
{
    private readonly IPublishEndpoint _publishEndpoint;

    public PublishEndpointService(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public Task PublishAsync<T>(T message, CancellationToken cancellationToken) where T : class
    {
        return _publishEndpoint.Publish(message, cancellationToken);
    }
}
