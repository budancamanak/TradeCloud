using Microsoft.Extensions.Logging;

namespace Common.Logging.Events.Market;

public static class MarketLogEvents
{
    public static readonly EventId GetPricesForPluginQuery = new EventId(1, "GetPricesForPluginQueryRequest");
    public static readonly EventId PriceFetchCompleted = new EventId(2, "PriceFetchCompletedCommand");
}