using Common.Core.Enums;

namespace Common.Core.DTOs.Backend;

public class PluginExecutionsDto
{
    public int Id { get; set; }
    public string PluginName { get; set; }
    public string PluginIdentifier { get; set; }
    public string Symbol { get; set; }
    public string Timeframe { get; set; }
    public string Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public string ParamSet { get; set; }
}