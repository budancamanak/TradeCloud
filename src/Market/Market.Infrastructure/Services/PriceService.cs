using AutoMapper;
using Common.Application.Repositories;
using Common.Core.DTOs;
using Common.Core.Enums;
using Common.Core.Models;
using Market.Application.Abstraction.Repositories;
using Market.Application.Abstraction.Services;
using Market.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Market.Infrastructure.Services;

/// <summary>
/// Responsible for fetching prices.
///     Will check for cache if prices exist. if so, will return it.<br />
///     if cache miss, will use repository and ask from db. will check for all prices existence. <br />
///     if all prices exist on db, will cache them and return. <br />
///     will start a pull job to fetch missing prices. <br />
///     - will calculate missing price timestamps   <br />
///         - will trigger fetch job for missing timestamps <br />
///     will have method that will be called by the fetch job   <br />
///         - will store fetched prices to database <br />
///         - will trigger kafka message to notify worker about price fetch status  <br />
///         * fetch job will pull price info from exchanges <br />
///         * fetch job will pass price info to price service   <br />
/// </summary>
/// <param name="cache"></param>
/// <param name="repository"></param>
public class PriceService(
    ICacheService cache,
    IPriceRepository repository,
    IMapper mapper,
    ILogger<PriceService> logger) : IPriceService
{
    public async Task<List<PriceDto>> GetPricesForPluginAsync(string cacheKey, int pluginId, int tickerId,
        Timeframe timeframe, DateTime start, DateTime end)
    {
        var cachedPrices = await cache.GetAsync<List<PriceDto>>(cacheKey);
        if (cachedPrices is { Count: > 0 })
        {
            logger.LogInformation("Returning from cache for {PluginId} Price count: {Count}", pluginId,
                cachedPrices.Count);
            return cachedPrices;
        }

        var dbResults = await repository.GetTickerPricesAsync(tickerId, timeframe, start, end);
        logger.LogDebug("Get prices from repository for {PluginId}: count:{Count}", pluginId, dbResults.Count);
        var output = mapper.Map<List<PriceDto>>(dbResults);
        return output;
    }

    public async Task<MethodResponse> SaveMissingPricesAsync(IList<Price> priceInfo, string? pluginIdToCache)
    {
        // todo need retry policy to prevent recurrent price fetches.
        logger.LogInformation("Saving prices for {PluginIdCache}, priceCount:{Count}", pluginIdToCache,
            priceInfo?.Count);
        var mr = await repository.SavePricesAsync(priceInfo);
        logger.LogInformation("Price save for {PluginIdCache}, result:{Response}", pluginIdToCache, mr);
        if (!mr.IsSuccess) return mr;
        if (!string.IsNullOrWhiteSpace(pluginIdToCache))
            await _SetCacheAsync(priceInfo, pluginIdToCache);
        return mr;
    }

    public async Task<MethodResponse> CachePrices(IList<PriceDto> priceInfo, string cacheKey)
    {
        await _SetCacheAsync(priceInfo, cacheKey);
        return MethodResponse.Success("Cached");
    }

    private async Task _SetCacheAsync(IList<Price> priceInfo, string cacheKey)
    {
        // logger.LogWarning("Caching prices for plugin[{}]", pluginId);
        var output = mapper.Map<List<PriceDto>>(priceInfo);
        await _SetCacheAsync(output, cacheKey);
    }

    private async Task _SetCacheAsync(IList<PriceDto> priceInfo, string cacheKey)
    {
        // logger.LogWarning("Caching prices for plugin[{}]", pluginId);
        // var output = mapper.Map<List<PriceDto>>(priceInfo);
        // var alreadyCached = await cache.GetAsync<List<PriceDto>>(pluginId) ?? new List<PriceDto>();
        // alreadyCached.AddRange(output);
        // alreadyCached = alreadyCached.DistinctBy(f => f.Timestamp).ToList();
        logger.LogWarning("Last Cache Operation ::: {CacheKey} Price count: {Count}", cacheKey, priceInfo.Count);
        await cache.SetAsync(cacheKey, priceInfo, TimeSpan.FromDays(1));
    }
}