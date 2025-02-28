using Common.Messaging.Events;

namespace Common.Messaging.Events.PluginExecution;

public class PluginProgressEvent(int pluginId, double progress) : IntegrationEvent
{
    public int PluginId { get; set; } = pluginId;
    public double Progress { get; set; } = progress;
}