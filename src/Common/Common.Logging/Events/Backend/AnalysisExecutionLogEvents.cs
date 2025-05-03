using Microsoft.Extensions.Logging;

namespace Common.Logging.Events.Backend;

public static class AnalysisExecutionLogEvents
{
    public static readonly EventId AnalysisExecutionDetails = new EventId(1, "AnalysisExecutionDetailsRequest");
    public static readonly EventId CreateAnalysisExecution = new EventId(2, "CreateAnalysisExecutionRequest");
    public static readonly EventId ListActivePlugins = new EventId(3, "ListActivePluginsRequest");
    public static readonly EventId ListAvailablePlugins = new EventId(4, "ListAvailablePluginsRequest");
    public static readonly EventId RunAnalysisExecution = new EventId(5, "RunAnalysisExecutionRequest");
    public static readonly EventId StopAnalysisExecution = new EventId(6, "StopAnalysisExecutionRequest");
    public static readonly EventId UserAnalysisExecutionList = new EventId(7, "UserAnalysisExecutionListRequest");
}