namespace Common.Messaging.Events.PriceFetchEvents;

public class PriceFetchedFailedIntegrationEvent(int pluginId, string message) : PriceFetchedIntegrationEvent(pluginId)
{
    public string Message { get; set; } = message;
}