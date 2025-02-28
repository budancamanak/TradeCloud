using Backend.Application.Abstraction.Services;
using Common.Application.Repositories;
using Common.Application.Services;
using Common.Core.DTOs;
using Common.Grpc;

namespace Backend.Infrastructure.Services;

public class TickerService(ICacheService cache, GrpcTickerController.GrpcTickerControllerClient grpcClient)
    : ITickerService
{
    public async Task<List<TickerDto>> GetAvailableTickers()
    {
        var cached = await cache.GetAsync<List<TickerDto>>(CacheKeyGenerator.AvailableTickers());
        if (cached == null)
        {
            var response = await grpcClient.GetAvailableTickersAsync(new GrpcGetAvailableTickersRequest());
            if (response != null)
            {
                // todo use AutoMapper here
                cached = new List<TickerDto>();
                foreach (var ticker in response.Tickers)
                {
                    cached.Add(new TickerDto(ticker.Id, ticker.Name, ticker.Symbol, ticker.ExchangeName,
                        ticker.DecimalPoint));
                }

                await cache.SetAsync(CacheKeyGenerator.AvailableTickers(), cached, TimeSpan.MaxValue);
            }
        }

        return cached;
    }

    public async Task<TickerDto> GetTickerWithId(int id)
    {
        var cached = await cache.GetAsync<TickerDto>(CacheKeyGenerator.TickerKey(id));
        if (cached == null)
        {
            var response = await grpcClient.GetTickerWithIdAsync(new GrpcGetTickerWithIdRequest { TickerId = id });
            if (response != null)
            {
                // todo use AutoMapper here
                // todo don't cache here. it's market service's responsibility
                cached = new TickerDto(response.Id, response.Name, response.Symbol, response.ExchangeName,
                    response.DecimalPoint);
                await cache.SetAsync(CacheKeyGenerator.TickerKey(id), cached, TimeSpan.MaxValue);
            }
        }

        return cached;
    }

    public async Task<TickerDto> GetTickerWithSymbol(string symbol)
    {
        var cached = await cache.GetAsync<TickerDto>(CacheKeyGenerator.TickerKey(symbol));
        if (cached == null)
        {
            var response =
                await grpcClient.GetTickerWithSymbolAsync(new GrpcGetTickerWithSymbolRequest() { Symbol = symbol });
            if (response != null)
            {
                // todo use AutoMapper here
                cached = new TickerDto(response.Id, response.Name, response.Symbol, response.ExchangeName,
                    response.DecimalPoint);
                await cache.SetAsync(CacheKeyGenerator.TickerKey(symbol), cached, TimeSpan.MaxValue);
            }
        }

        return cached;
    }
}