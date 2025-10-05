using Common.Core.Enums;
using Common.Core.Models;

namespace Common.Messaging.Events.PluginExecution;

public class PluginStatusEvent(int pluginId, int analysisId, PluginStatus status, string error) : IntegrationEvent
{
    public int PluginId { get; set; } = pluginId;
    public int AnalysisId { get; set; } = analysisId;
    public PluginStatus Status { get; set; } = status;
    public string Error { get; set; } = error;
}