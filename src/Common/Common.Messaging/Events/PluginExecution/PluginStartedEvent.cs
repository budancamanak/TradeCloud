using Common.Messaging.Events;

namespace Common.Messaging.Events.PluginExecution;

public class PluginStartedEvent(string identifier) : IntegrationEvent
{
}