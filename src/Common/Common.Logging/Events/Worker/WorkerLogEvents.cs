using Microsoft.Extensions.Logging;

namespace Common.Logging.Events.Worker;

public static class WorkerLogEvents
{
    public static readonly EventId RunAnalysis = new EventId(1, "RunAnalysisRequest");
    public static readonly EventId WorkerCache = new EventId(2, "WorkerCache");
    public static readonly EventId PluginHost = new EventId(3, "PluginHost");
    public static readonly EventId PluginLoader = new EventId(4, "PluginLoader");
    public static readonly EventId PluginMessageBroker = new EventId(5, "PluginMessageBroker");
    public static readonly EventId GrpcWorkerAPI = new EventId(6, "GrpcWorkerAPI");
}