using Common.Core.Enums;

namespace Backend.Domain.Entities;

public class PluginExecution
{
    public int Id { get; set; }
    public PluginStatus Status { get; set; } = PluginStatus.Init;
    public double Progress { get; set; } = 0;
    public string ParamSet { get; set; }
    public string Error { get; set; } //todo make use of error field
    public DateTime? QueuedDate { get; set; }
    public DateTime? RunStartDate { get; set; }
    public DateTime? FinishStartDate { get; set; }
    public int AnalysisExecutionId { get; set; }
    public virtual ICollection<PluginOutput> PluginOutputs { get; set; }
    public virtual AnalysisExecution AnalysisExecution { get; set; }

    public override string ToString()
    {
        return $"[{Id}] AnalysisExecutionId:{AnalysisExecutionId}, Status:{Status}, Params:{ParamSet}";
    }
}