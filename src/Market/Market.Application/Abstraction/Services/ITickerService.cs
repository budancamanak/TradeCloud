using Market.Application.Features.GetPricesForPlugin.Request;
using Market.Application.Models;

namespace Market.Application.Abstraction.Services;

public interface ITickerService
{
    // public Task<PriceFetchRequest> CreateFetchRequest(int pluginId, string cacheKey, int tickerId, Timeframe timeFrame);
    public Task<PriceFetchRequest> CreateFetchRequest(GetPricesForPluginQuery query);
}