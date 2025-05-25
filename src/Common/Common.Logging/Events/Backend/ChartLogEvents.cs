using Microsoft.Extensions.Logging;

namespace Common.Logging.Events.Backend;

public class ChartLogEvents
{
    public static readonly EventId ExecutionPrices = new EventId(1, "ExecutionPricesRequest");
}