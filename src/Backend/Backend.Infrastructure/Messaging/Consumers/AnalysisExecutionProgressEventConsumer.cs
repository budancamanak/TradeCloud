using Backend.Application.Abstraction.Repositories;
using Common.Messaging.Events.PluginExecution;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Backend.Infrastructure.Messaging.Consumers;

public class AnalysisExecutionProgressEventConsumer(
    IAnalysisExecutionRepository repository,
    ILogger<AnalysisExecutionProgressEventConsumer> logger) : IConsumer<AnalysisExecutionProgressEvent>
{
    public async Task Consume(ConsumeContext<AnalysisExecutionProgressEvent> context)
    {
        // logger.LogInformation("Consuming PluginProgressEvent > Setting plugin[{}] progress to {} ",
        //     context.Message.PluginId, context.Message.Progress);
        var mr = await repository.SetAnalysisExecutionProgress(context.Message.AnalysisExecutionId, context.Message.Increment,
            context.Message.Total);
        // logger.LogInformation("Consumed PluginProgressEvent > Setting plugin[{}] progress to {} : {}",
        //     context.Message.PluginId, context.Message.Progress, mr);
    }
}