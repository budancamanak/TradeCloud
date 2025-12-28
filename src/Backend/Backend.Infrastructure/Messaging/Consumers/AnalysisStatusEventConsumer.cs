using Backend.Application.Abstraction.Repositories;
using Backend.Infrastructure.Services;
using Common.Messaging.Events.PluginExecution;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Backend.Infrastructure.Messaging.Consumers;

public class AnalysisStatusEventConsumer(
    IProgressNotifier _notifier,
    ILogger<AnalysisStatusEventConsumer> logger)
    : IConsumer<AnalysisStatusEvent>
{
    public async Task Consume(ConsumeContext<AnalysisStatusEvent> context)
    {
        logger.LogInformation(
            "Consuming AnalysisStatusEvent > Setting analysis[{PluginId}] status to {Status}",
            context.Message.AnalysisId, context.Message.Status);

        await _notifier.NotifyStatusChangedAsync(context.Message.AnalysisId, context.Message.Status.ToString());
    }
}