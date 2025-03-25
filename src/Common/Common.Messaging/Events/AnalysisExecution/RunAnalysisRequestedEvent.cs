namespace Common.Messaging.Events.AnalysisExecution;

public class RunAnalysisRequestedEvent : IntegrationEvent
{
    public int ExecutionId { get; set; }
    public string Identifier { get; set; }
    public int Ticker { get; set; }
    public string Timeframe { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime StartDate { get; set; }
    public List<RunPluginInfo> PluginInfos { get; set; }
}

public class RunPluginInfo
{
    public int PluginExecutionId { get; set; }
    public string PluginParameters { get; set; }
}