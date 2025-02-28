using Common.Core.Models;

namespace Common.Messaging.Events.PluginExecution;

public class RunPluginRequestedEvent : IntegrationEvent
{
    public int ExecutionId { get; set; }
    public string Identifier { get; set; }
    public int Ticker { get; set; }
    public string Timeframe { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime StartDate { get; set; }
}