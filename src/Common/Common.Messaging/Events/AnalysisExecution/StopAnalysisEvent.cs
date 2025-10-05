using Common.Core.Models;

namespace Common.Messaging.Events.AnalysisExecution;

public class StopAnalysisEvent : IntegrationEvent
{
    public int[] PluginExecutionIds { get; set; }
}