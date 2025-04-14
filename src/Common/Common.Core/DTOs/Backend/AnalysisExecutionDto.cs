using Common.Core.Models;

namespace Common.Core.DTOs.Backend;

public class AnalysisExecutionDto
{
    public int Id { get; set; }
    public string Status { get; set; }
    public PluginInfo PluginInfo { get; set; }
    public PluginExecutionsDto[] PluginExecutions { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}