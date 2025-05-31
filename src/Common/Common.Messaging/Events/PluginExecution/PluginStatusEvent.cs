using Common.Core.Enums;

namespace Common.Messaging.Events.PluginExecution;

public class PluginStatusEvent(int pluginId,int analysisId, PluginStatus status) : IntegrationEvent
{
    public int PluginId { get; set; } = pluginId;
    public int AnalysisId { get; set; } = analysisId;
    public PluginStatus Status { get; set; } = status;
}