using Ardalis.GuardClauses;
using AutoMapper;
using Common.Application.Repositories;
using Common.Application.Services;
using Common.Core.DTOs;
using Market.Application.Abstraction.Repositories;
using Market.Application.Abstraction.Services;
using Market.Application.Features.GetPricesForPlugin.Request;
using Market.Application.Models;
using Microsoft.Extensions.Logging;

namespace Market.Infrastructure.Services;

//todo rename this??
public class TickerService(
    ICacheService cache,
    ITickerRepository tickerRepository,
    IMapper mapper,
    ILogger<TickerService> logger) : ITickerService
{
    // public async Task<PriceFetchRequest> CreateFetchRequest(int pluginId, string cacheKey, int tickerId,
    //     Timeframe timeFrame)
    // {
    //     Guard.Against.NegativeOrZero(tickerId);
    //     Guard.Against.EnumOutOfRange(timeFrame);
    //     var cachedTicker = await cache.GetAsync<TickerDto>(tickerId.ToString());
    //     if (cachedTicker != null)
    //         return new PriceFetchRequest(pluginId, cacheKey, tickerId, "binance", cachedTicker.Symbol, timeFrame);
    //
    //     var ticker = await tickerRepository.GetByIdAsync(tickerId);
    //     Guard.Against.Null(ticker);
    //     cachedTicker = mapper.Map<TickerDto>(ticker);
    //     await cache.SetAsync(tickerId.ToString(), cachedTicker, TimeSpan.FromDays(30));
    //
    //     return new PriceFetchRequest(pluginId, cacheKey, tickerId, ticker.Exchange.Name, cachedTicker.Symbol,
    //         timeFrame);
    // }

    public async Task<PriceFetchRequest> CreateFetchRequest(GetPricesForPluginQuery query)
    {
        Guard.Against.NegativeOrZero(query.TickerId);
        Guard.Against.EnumOutOfRange(query.Timeframe);
        var cacheKey = CacheKeyGenerator.TickerKey(query.TickerId);
        var cachedTicker = await cache.GetAsync<TickerDto>(cacheKey);
        if (cachedTicker != null)
            return new PriceFetchRequest(query.PluginId, query.CacheKey, query.TickerId, cachedTicker.ExchangeName,
                cachedTicker.Symbol, query.Timeframe);

        var ticker = await tickerRepository.GetByIdAsync(query.TickerId);
        Guard.Against.Null(ticker);
        cachedTicker = mapper.Map<TickerDto>(ticker);
        await cache.SetAsync(cacheKey, cachedTicker, TimeSpan.FromDays(30));

        return new PriceFetchRequest(query.PluginId, query.CacheKey, query.TickerId, ticker.Exchange.Name,
            cachedTicker.Symbol, query.Timeframe);
    }
}