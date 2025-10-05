using System.Threading.Channels;
using Common.Core.Models;

namespace Common.Application.Queue;

// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-9.0&tabs=visual-studio
public class BackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly Channel<IntegrationEvent> _queue;

    public BackgroundTaskQueue(int capacity)
    {
        // Capacity should be set based on the expected application load and
        // number of concurrent threads accessing the queue.            
        // BoundedChannelFullMode.Wait will cause calls to WriteAsync() to return a task,
        // which completes only when space became available. This leads to backpressure,
        // in case too many publishers/calls start accumulating.
        var options = new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        };
        _queue = Channel.CreateBounded<IntegrationEvent>(options);
    }

    public async ValueTask QueueBackgroundWorkItemAsync(IntegrationEvent workItem)
    {
        if (workItem == null)
        {
            throw new ArgumentNullException(nameof(workItem));
        }

        await _queue.Writer.WriteAsync(workItem);
    }

    public async ValueTask<IntegrationEvent> DequeueAsync(CancellationToken cancellationToken)
    {
        var workItem = await _queue.Reader.ReadAsync(cancellationToken);

        return workItem;
    }
}