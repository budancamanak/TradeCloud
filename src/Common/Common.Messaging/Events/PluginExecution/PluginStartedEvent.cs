using Common.Core.Models;

namespace Common.Messaging.Events.PluginExecution;

public class PluginStartedEvent(string identifier) : IntegrationEvent
{
}