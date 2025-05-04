using Microsoft.Extensions.Logging;

namespace Common.Logging.Events;

public static class MQEvents
{
    public static readonly EventId PriceFetchedEvent = new EventId(1, "PriceFetchedIntegrationEvent");
    public static readonly EventId RunAnalysisRequestedEvent = new EventId(2, "RunAnalysisRequestedEvent");
    public static readonly EventId PriceFetchedFailedEvent = new EventId(3, "PriceFetchedFailedIntegrationEvent");
    public static readonly EventId StopAnalysisEvent = new EventId(4, "StopAnalysisEvent");
}