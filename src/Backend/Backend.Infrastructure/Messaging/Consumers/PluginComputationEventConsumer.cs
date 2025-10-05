using Common.Application.Queue;
using Common.Messaging.Events.PluginExecution;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Backend.Infrastructure.Messaging.Consumers;

public class PluginComputationEventConsumer(
    IBackgroundTaskQueue taskQueue,
    ILogger<PluginComputationEventConsumer> logger) : IConsumer<PluginComputationEvent>
{
    public async Task Consume(ConsumeContext<PluginComputationEvent> context)
    {
        logger.LogInformation("PluginComputationEvent:{Computation}", context.Message.ToString());
        await taskQueue.QueueBackgroundWorkItemAsync(context.Message);
    }
}