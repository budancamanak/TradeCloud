using Backend.Application.Abstraction.Repositories;
using Backend.Domain.Entities;
using Common.Application.Queue;
using Common.Core.Models;
using Common.Logging.Events.Backend;
using Common.Messaging.Events.PluginExecution;
using Microsoft.Extensions.Logging;

namespace Backend.Infrastructure.Services;

public class PluginProgressService(
    IPluginExecutionRepository pluginExecutionRepository,
    IBackgroundTaskQueue taskQueue,
    ILogger<PluginProgressService> logger) : QueuedHostedService(taskQueue, logger)
{
    protected override async Task ExecuteItem(IntegrationEvent workItem)
    {
        if (workItem is not PluginProgressEvent model) return;
        logger.LogDebug(ChartLogEvents.ExecutionProgress, "Consuming {0}", $"{model.PluginId}-%{model.Progress}");
        await pluginExecutionRepository.SetPluginProgress(model.PluginId,model.Progress);
    }
}