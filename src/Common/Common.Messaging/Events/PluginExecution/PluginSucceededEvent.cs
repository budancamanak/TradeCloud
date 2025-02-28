using Common.Messaging.Events;

namespace Common.Messaging.Events.PluginExecution;

public class PluginSucceededEvent(string identifier) : IntegrationEvent
{
}