using Microsoft.Extensions.Logging;

namespace Common.Logging.Events.Worker;

public static class WorkerLogEvents
{
    public static readonly EventId RunAnalysis = new EventId(1, "RunAnalysisRequest");
    public static readonly EventId WorkerCache = new EventId(2, "WorkerCache");
}