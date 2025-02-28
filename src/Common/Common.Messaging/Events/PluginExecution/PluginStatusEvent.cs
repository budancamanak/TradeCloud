using Common.Core.Enums;

namespace Common.Messaging.Events.PluginExecution;

public class PluginStatusEvent(int pluginId, PluginStatus status) : IntegrationEvent
{
    public int PluginId { get; set; } = pluginId;
    public PluginStatus Status { get; set; } = status;
}