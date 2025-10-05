using Common.Core.Models;

namespace Common.Messaging.Events.PriceFetchEvents;

public class PriceFetchedIntegrationEvent(int pluginId) : IntegrationEvent
{
    public int PluginId { get; set; } = pluginId;
}