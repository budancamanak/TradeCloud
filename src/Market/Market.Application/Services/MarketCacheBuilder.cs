using AutoMapper;
using Common.Application.Repositories;
using Common.Application.Services;
using Common.Core.DTOs;
using Market.Application.Abstraction.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Market.Application.Services;

public class MarketCacheBuilder(
    [FromKeyedServices("ticker")] ICacheBuilder tickerBuilder,
    [FromKeyedServices("exchange")] ICacheBuilder exchangeBuilder) : ICacheBuilder
{
    public async Task BuildCacheAsync()
    {
        await tickerBuilder.BuildCacheAsync();
        await exchangeBuilder.BuildCacheAsync();
    }
}

public class ExchangeCacheBuilder(
    ICacheService cache,
    IExchangeRepository repository,
    IMapper mapper,
    ILogger<TickerCacheBuilder> logger)
    : ICacheBuilder
{
    public async Task BuildCacheAsync()
    {
        logger.LogInformation("Building exchange cache...");
        var exchanges = await repository.GetAllAsync();
        var dtos = mapper.Map<List<ExchangeDto>>(exchanges);
        foreach (var exchangeDto in dtos)
        {
            var cacheKey = CacheKeyGenerator.ExchangeKey(exchangeDto.Id);
            var cached = await cache.GetAsync<ExchangeDto>(cacheKey);
            if (cached != null) continue;
            logger.LogInformation("Adding exchange{} to cache", exchangeDto);
            await cache.SetAsync<ExchangeDto>(cacheKey, exchangeDto, TimeSpan.FromDays(30));
        }
    }
}

public class TickerCacheBuilder(
    ICacheService cache,
    ITickerRepository repository,
    IMapper mapper,
    ILogger<TickerCacheBuilder> logger)
    : ICacheBuilder
{
    public async Task BuildCacheAsync()
    {
        logger.LogInformation("Building ticker cache...");
        var tickers = await repository.GetAllAsync();
        var dtos = mapper.Map<List<TickerDto>>(tickers);
        await cache.SetAsync(CacheKeyGenerator.AvailableTickers(), dtos, TimeSpan.MaxValue);
        foreach (var tickerDto in dtos)
        {
            var cacheKey = CacheKeyGenerator.TickerKey(tickerDto.Id);
            var cached = await cache.GetAsync<TickerDto>(cacheKey);
            if (cached == null)
            {
                logger.LogInformation("Adding ticker{} to cache with {}", tickerDto, cacheKey);
                await cache.SetAsync<TickerDto>(cacheKey, tickerDto, TimeSpan.FromDays(30));
            }

            cacheKey = CacheKeyGenerator.TickerKey(tickerDto.Symbol);
            cached = await cache.GetAsync<TickerDto>(cacheKey);
            if (cached == null)
            {
                logger.LogInformation("Adding ticker{} to cache with {}", tickerDto, cacheKey);
                await cache.SetAsync<TickerDto>(cacheKey, tickerDto, TimeSpan.FromDays(30));
            }
        }
    }
}