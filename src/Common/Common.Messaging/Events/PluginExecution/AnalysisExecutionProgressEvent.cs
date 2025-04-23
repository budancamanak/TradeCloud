namespace Common.Messaging.Events.PluginExecution;

public class AnalysisExecutionProgressEvent(int analysisExecutionId, int increment, int total) : IntegrationEvent
{
    public int AnalysisExecutionId { get; set; } = analysisExecutionId;
    public int Increment { get; set; } = increment;
    public int Total { get; set; } = total;
}