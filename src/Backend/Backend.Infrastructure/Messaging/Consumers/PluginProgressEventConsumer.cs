using Backend.Application.Abstraction.Repositories;
using Common.Messaging.Events.PluginExecution;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Backend.Infrastructure.Messaging.Consumers;

public class PluginProgressEventConsumer(
    IPluginExecutionRepository repository,
    ILogger<PluginProgressEventConsumer> logger) : IConsumer<PluginProgressEvent>
{
    public async Task Consume(ConsumeContext<PluginProgressEvent> context)
    {
        // logger.LogInformation("Consuming PluginProgressEvent > Setting plugin[{}] progress to {} ",
        //     context.Message.PluginId, context.Message.Progress);
        var mr = await repository.SetPluginProgress(context.Message.PluginId, context.Message.Progress);
        // logger.LogInformation("Consumed PluginProgressEvent > Setting plugin[{}] progress to {} : {}",
        //     context.Message.PluginId, context.Message.Progress, mr);
    }
}