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
        logger.LogInformation("Consuming stop analysis event");
        foreach (var executionId in context.Message.PluginExecutionIds)
        {
            logger.LogInformation("Processing stop analysis event: {}", executionId);
            stateManager.OnPluginStopped(executionId);
        }

        return Task.CompletedTask;
    }
}