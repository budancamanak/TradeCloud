using Common.Logging.Events;
using Common.Messaging.Events.AnalysisExecution;
using Common.Plugin.Abstraction;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Worker.Infrastructure.Messaging.Consumers;

public class StopAnalysisExecutionConsumer(
    IPluginStateManager stateManager,
    ILogger<StopAnalysisExecutionConsumer> logger) : IConsumer<StopAnalysisEvent>
{
    public Task Consume(ConsumeContext<StopAnalysisEvent> context)
    {
        logger.LogInformation(MQEvents.StopAnalysisEvent,
            "Run analysis event requested for PluginIds[{AnalysisExecution}] @ {Date}",
            string.Join(",", context.Message.PluginExecutionIds), context.Message.CreatedDate);
        logger.LogInformation("Consuming stop analysis event");
        foreach (var executionId in context.Message.PluginExecutionIds)
        {
            logger.LogInformation(MQEvents.StopAnalysisEvent, "Processing stop analysis event: PluginId[{PluginId}]",
                executionId);
            stateManager.OnPluginStopped(executionId);
        }

        return Task.CompletedTask;
    }
}