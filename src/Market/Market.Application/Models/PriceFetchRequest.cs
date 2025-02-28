using Common.Core.Enums;

namespace Market.Application.Models;

public record PriceFetchRequest(
    int PluginId,
    string CacheKey,
    int TickerId,
    string ExchangeName,
    string Symbol,
    Timeframe Timeframe)
{
}