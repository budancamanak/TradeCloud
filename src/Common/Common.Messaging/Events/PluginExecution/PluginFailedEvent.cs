using Common.Messaging.Events;

namespace Common.Messaging.Events.PluginExecution;

public class PluginFailedEvent(string identifier, Exception exception) : IntegrationEvent
{
}