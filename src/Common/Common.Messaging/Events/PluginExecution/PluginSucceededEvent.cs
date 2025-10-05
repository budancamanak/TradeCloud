using Common.Core.Models;

namespace Common.Messaging.Events.PluginExecution;

public class PluginSucceededEvent(string identifier) : IntegrationEvent
{
}