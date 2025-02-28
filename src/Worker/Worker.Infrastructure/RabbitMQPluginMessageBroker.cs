using Common.Core.Enums;
using Common.Messaging.Abstraction;
using Common.Messaging.Events.PluginExecution;
using Common.Plugin.Abstraction;
using Common.Plugin.Signals;
using Microsoft.Extensions.Logging;

namespace Worker.Infrastructure;

public class RabbitMQPluginMessageBroker(IEventBus eventBus, ILogger<RabbitMQPluginMessageBroker> logger)
    : IPluginMessageBroker
{
    public async void OnPluginStarted(IPlugin plugin, int executionId)
    {
        logger.LogWarning("OnPluginStarted: {}", plugin.GetPluginInfo());
        await eventBus.PublishAsync(new PluginStatusEvent(executionId, PluginStatus.Running));
        //await eventBus.PublishAsync(new PluginStartedEvent(plugin.GetPluginInfo().Identifier));
    }

    public async void OnPluginSucceeded(IPlugin plugin, int executionId)
    {
        logger.LogWarning("OnPluginSucceeded: {}", plugin.GetPluginInfo());
        // await eventBus.PublishAsync(new PluginProgressEvent(executionId, 1.0d));
        await eventBus.PublishAsync(new PluginStatusEvent(executionId, PluginStatus.Success));
        // await eventBus.PublishAsync(new PluginSucceededEvent(plugin.GetPluginInfo().Identifier));
    }

    public async void OnPluginFailed(IPlugin plugin, int executionId, Exception exception)
    {
        logger.LogWarning("OnPluginFailed: {} - {}", plugin.GetPluginInfo(), exception);
        await eventBus.PublishAsync(new PluginStatusEvent(executionId, PluginStatus.Failure));
        // await eventBus.PublishAsync(
        //     new PluginFailedEvent(plugin.GetPluginInfo().Identifier, exception));
    }

    public async void OnPluginProgress(IPlugin plugin, int executionId, int current, int total)
    {
        // logger.LogWarning("OnPluginProgress:{}/{} %{} -> {}", current, total, (double)current / total,
        //     plugin.GetPluginInfo());
        await eventBus.PublishAsync(new PluginProgressEvent(executionId, (double)current / total));
    }

    public async void OnPluginSignal(IPlugin plugin, int executionId, PluginSignal signal)
    {
        logger.LogWarning("OnPluginSignal:{} {}", signal, plugin.GetPluginInfo());
        await eventBus.PublishAsync(new PluginSignalEvent(executionId, signal));
    }

    public void Log(string arg)
    {
        logger.LogWarning("Log from plugin: {}", arg);
    }
}