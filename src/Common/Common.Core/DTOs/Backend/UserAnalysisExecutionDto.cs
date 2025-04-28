using Common.Core.Models;

namespace Common.Core.DTOs.Backend;

public class UserAnalysisExecutionDto
{
    public int Id { get; set; }
    public string Ticker { get; set; }
    public string Timeframe { get; set; }
    public string Plugin { get; set; }
    public string Status { get; set; }
    public double Progress { get; set; }
    public string ParamSet { get; set; }
    public string TradingParams { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public PluginExecutionsDto[] PluginExecutions { get; set; }
}