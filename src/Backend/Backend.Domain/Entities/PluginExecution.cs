using Common.Core.Enums;

namespace Backend.Domain.Entities;

public class PluginExecution
{
    public int Id { get; set; }
    public string PluginIdentifier { get; set; }
    public int TickerId { get; set; }
    public Timeframe Timeframe { get; set; }
    public PluginStatus Status { get; set; } = PluginStatus.Init;
    public double Progress { get; set; } = 0;
    public int UserId { get; set; }
    public string ParamSet { get; set; }
    public string TradingParams { get; set; }
    public string Error { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<PluginOutput> PluginOutputs { get; set; }

    public override string ToString()
    {
        return $"[{Id}] TickerId:{TickerId} {Timeframe} [{Status}] [{PluginIdentifier}]";
    }
}