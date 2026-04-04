namespace JobApplicationTracker.Application.Common.Interfaces;

public interface IPublishEndpointService
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken) where T : class;
}
