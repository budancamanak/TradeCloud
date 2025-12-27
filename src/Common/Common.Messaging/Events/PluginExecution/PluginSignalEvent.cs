using Common.Core.Models;
using Common.Plugin.Signals;

namespace Common.Messaging.Events.PluginExecution;

public class PluginSignalEvent(int pluginId, PluginSignal signal) : IntegrationEvent
{
    public int PluginId { get; set; } = pluginId;
    public PluginSignal Signal { get; set; } = signal;

    public override string ToString()
    {
        return $"{PluginId} - {Signal}";
    }
}