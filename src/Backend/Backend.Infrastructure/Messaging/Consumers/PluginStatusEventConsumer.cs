using Backend.Application.Abstraction.Repositories;
using Common.Core.Enums;
using Common.Messaging.Events.PluginExecution;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Backend.Infrastructure.Messaging.Consumers;

public class PluginStatusEventConsumer(IPluginExecutionRepository repository, ILogger<PluginStatusEventConsumer> logger)
    : IConsumer<PluginStatusEvent>
{
    public async Task Consume(ConsumeContext<PluginStatusEvent> context)
    {
        logger.LogInformation("Consuming PluginStatusEvent > Setting plugin[{}] status to {} ",
            context.Message.PluginId, context.Message.Status);
        var mr = await repository.SetPluginStatus(context.Message.PluginId, context.Message.Status);
        if (context.Message.Status == PluginStatus.Success)
        {
            logger.LogInformation("PluginStatusEvent > Setting plugin[{}] progress to {}",
                context.Message.PluginId, 1.0d);
            mr = await repository.SetPluginProgress(context.Message.PluginId, 1.0d);
        }

        logger.LogInformation("Consumed PluginStatusEvent > Setting plugin[{}] status to {} : {}",
            context.Message.PluginId, context.Message.Status, mr);
    }
}