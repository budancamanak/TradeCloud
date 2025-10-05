using Common.Core.Models;

namespace Common.Messaging.Events.PluginExecution;

public class PluginFailedEvent(string identifier, Exception exception) : IntegrationEvent
{
}