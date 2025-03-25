namespace Backend.API.Models;

public class CreateAnalysisExecutionModel
{
    public string PluginIdentifier { get; set; }
    public string Symbol { get; set; }
    public string Timeframe { get; set; }
    public string ParamSet { get; set; }
    public string TradingParams { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}