using Backend.Application.Abstraction.Repositories;
using Common.Core.Enums;
using Common.Messaging.Events.PluginExecution;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Backend.Infrastructure.Messaging.Consumers;

public class PluginStatusEventConsumer(
    IPluginExecutionRepository repository,
    IAnalysisExecutionRepository analysisRepository,
    ILogger<PluginStatusEventConsumer> logger)
    : IConsumer<PluginStatusEvent>
{
    public async Task Consume(ConsumeContext<PluginStatusEvent> context)
    {
        logger.LogInformation(
            "Consuming PluginStatusEvent > Setting plugin[{PluginId}] status to {Status} Error:[{Error}]",
            context.Message.PluginId, context.Message.Status, context.Message.Error);
        var mr = await repository.SetPluginStatus(context.Message.PluginId, context.Message.Status);
        if (context.Message.Status is PluginStatus.Success or PluginStatus.Failure)
        {
            // logger.LogInformation("PluginStatusEvent > Setting plugin[{PluginId}] progress to {Status}",
            //     context.Message.PluginId, 1.0d);
            // mr = await repository.SetPluginProgress(context.Message.PluginId, 1.0d);
            await analysisRepository.SetAnalysisExecutionProgress(context.Message.AnalysisId, 1, 0);
            if (context.Message.Status == PluginStatus.Failure)
            {
                await repository.SetPluginError(context.Message.PluginId, context.Message.Error);
            }
        }


        logger.LogInformation("Consumed PluginStatusEvent > Setting plugin[{PluginId}] status to {Status} : {Result}",
            context.Message.PluginId, context.Message.Status, mr);
    }
}