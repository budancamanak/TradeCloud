using Common.Core.Models;

namespace Common.Application.Queue;

// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-9.0&tabs=visual-studio
public interface IBackgroundTaskQueue
{
    ValueTask QueueBackgroundWorkItemAsync(IntegrationEvent workItem); 

    ValueTask<IntegrationEvent> DequeueAsync(
        CancellationToken cancellationToken);
}