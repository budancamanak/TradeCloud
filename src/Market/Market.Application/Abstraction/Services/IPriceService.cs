using Common.Core.DTOs;
using Common.Core.Enums;
using Common.Core.Models;
using Market.Domain.Entities;

namespace Market.Application.Abstraction.Services;

public interface IPriceService
{
    /// <summary>
    /// Will get price info. Will check for cache first, then goes to database
    /// Will cache all returned prices from database 
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <param name="pluginId">caller plugin's id to check and set cache</param>
    /// <param name="tickerId"></param>
    /// <param name="timeframe"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">Throws when tickerId is negative or zero</exception>
    /// <exception cref="ArgumentNullException">Throws when startDate has default value</exception>
    public Task<List<PriceDto>> GetPricesForPluginAsync(string cacheKey, int pluginId, int tickerId,
        Timeframe timeframe,
        DateTime start, DateTime end);

    /// <summary>
    /// Will save price info to database and merge with the results that are on cache.
    /// </summary>
    /// <param name="priceInfo"></param>
    /// <param name="pluginIdToCache"></param>
    /// <returns></returns>
    public Task<MethodResponse> SaveMissingPricesAsync(IList<Price> priceInfo, string? pluginIdToCache);

    public Task<MethodResponse> CachePrices(IList<PriceDto> priceInfo, string pluginIdToCache);
}