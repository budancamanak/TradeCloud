using Common.Core.Enums;
using Common.Core.Models;

namespace Common.Messaging.Events.PluginExecution;

public class AnalysisStatusEvent(int analysisId, PluginStatus status) : IntegrationEvent
{
    public int AnalysisId { get; set; } = analysisId;
    public PluginStatus Status { get; set; } = status;
}