namespace Common.Core.DTOs.Backend;

public class PluginExecutionsDto
{
    public int Id { get; set; }

    // todo values below could be fetched by using AnalysisExecutionId and a service
    // public string PluginName { get; set; }
    // public string PluginIdentifier { get; set; }
    // public string Symbol { get; set; }
    // public string Timeframe { get; set; }
    public string Status { get; set; }
    public double Progress { get; set; }
    public DateTime? QueuedDate { get; set; }
    public DateTime? RunStartDate { get; set; }
    public DateTime? FinishDate { get; set; }
    public string ParamSet { get; set; }
    public string Error { get; set; }
    public PluginOutputDto[] Outputs { get; set; } = [];
}