using Common.Core.Enums;
using Common.Logging.Events.Worker;
using Common.Messaging.Abstraction;
using Common.Messaging.Events.PluginExecution;
using Common.Plugin.Abstraction;
using Common.Plugin.Signals;
using Microsoft.Extensions.Logging;

namespace Worker.Infrastructure;

public class RabbitMQPluginMessageBroker(IEventBus eventBus, ILogger<RabbitMQPluginMessageBroker> logger)
    : IPluginMessageBroker
{
    public async Task OnPluginStarted(IPlugin plugin, int executionId)
    {
        logger.LogWarning(WorkerLogEvents.PluginMessageBroker, "OnPluginStarted: {PluginInfo}", plugin.GetPluginInfo());
        await eventBus.PublishAsync(new PluginStatusEvent(executionId, PluginStatus.Running));
    }

    public async Task OnPluginSucceeded(IPlugin plugin, int executionId)
    {
        logger.LogWarning(WorkerLogEvents.PluginMessageBroker, "OnPluginSucceeded: {PluginInfo}",
            plugin.GetPluginInfo());
        await eventBus.PublishAsync(new PluginStatusEvent(executionId, PluginStatus.Success));
    }

    public async Task OnPluginFailed(IPlugin plugin, int executionId, Exception exception)
    {
        logger.LogWarning(WorkerLogEvents.PluginMessageBroker, "OnPluginFailed: {PluginInfo} - Reason:{Reason}",
            plugin.GetPluginInfo(), exception);
        await eventBus.PublishAsync(new PluginStatusEvent(executionId, PluginStatus.Failure));
    }

    public async Task OnPluginProgress(IPlugin plugin, int executionId, int current, int total)
    {
        logger.LogDebug(WorkerLogEvents.PluginMessageBroker,
            "OnPluginProgress:{Current}/{Total} %{Percentage} -> {PluginInfo}", current, total,
            (double)current / total,
            plugin.GetPluginInfo());
        await eventBus.PublishAsync(new PluginProgressEvent(executionId, (double)current / total));
    }

    public async Task OnAnalysisProgress(IPlugin plugin, int analysisExecution, int increment, int total)
    {
        logger.LogDebug(WorkerLogEvents.PluginMessageBroker,
            "OnPluginProgress:{Total}  -> {PluginInfo}", total,
            plugin.GetPluginInfo());
        await eventBus.PublishAsync(new AnalysisExecutionProgressEvent(analysisExecution, increment, total));
    }

    public async Task OnPluginSignal(IPlugin plugin, int executionId, PluginSignal signal)
    {
        logger.LogWarning(WorkerLogEvents.PluginMessageBroker, "OnPluginSignal:{Signal} for {PluginInfo}", signal,
            plugin.GetPluginInfo());
        await eventBus.PublishAsync(new PluginSignalEvent(executionId, signal));
    }
}