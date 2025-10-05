using Backend.Application.Abstraction.Repositories;
using Common.Application.Queue;
using Common.Application.Services;
using Common.Core.Models;
using Common.Logging.Events.Backend;
using Common.Messaging.Events.PluginExecution;
using Microsoft.Extensions.Logging;

namespace Backend.Infrastructure.Services;

public class PluginComputationService(
    IPluginExecutionRepository pluginExecutionRepository,
    ICollector collector,
    IBackgroundTaskQueue taskQueue,
    ILogger<PluginComputationService> logger) : QueuedHostedService(taskQueue, logger)
{
    protected override async Task ExecuteItem(IntegrationEvent workItem)
    {
        if (workItem is not PluginComputationEvent model) return;
        logger.LogDebug(ChartLogEvents.ExecutionProgress, "Consuming {Computation}", model.ToString());
        // await pluginExecutionRepository.SetPluginProgress(model.PluginId,model.Progress);
        await Task.Run(() => { collector.Collect(model); });
    }
}