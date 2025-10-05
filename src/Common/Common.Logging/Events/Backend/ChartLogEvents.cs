using Microsoft.Extensions.Logging;

namespace Common.Logging.Events.Backend;

public class ChartLogEvents
{
    public static readonly EventId ExecutionPrices = new EventId(1, "ExecutionPricesRequest");
    public static readonly EventId ExecutionSignal = new EventId(2, "ExecutionSignalRequest");
    public static readonly EventId ExecutionProgress = new EventId(3, "ExecutionProgressRequest");
}