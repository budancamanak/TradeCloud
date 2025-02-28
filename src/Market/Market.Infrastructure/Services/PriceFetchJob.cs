using Ardalis.GuardClauses;
using Common.Core.DTOs;
using Common.Core.Enums;
using Common.Core.Models;
using Market.Application.Abstraction.Services;
using Market.Application.Exceptions;
using Market.Application.Models;
using Market.Application.Features.SaveFetchedPrices.Request;
using Market.Application.Utilities;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Market.Infrastructure.Services;

public class PriceFetchJob(IMediator mediator, ILogger<PriceFetchJob> logger) : IPriceFetchJob
{
    private int _lastFetchedPage = 0;
    public List<PriceDto> FetchedPrices { get; set; } = new();
    public PriceFetchRequest? ActiveRequest { get; set; }

    public async Task<MethodResponse> StartFetchPrices(IList<PriceFetchPages> pages, PriceFetchRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"Started fetch operation for {request}");
        ActiveRequest = request;
        var exchangeInstance = ccxt.Exchange.DynamicallyCreateInstance(request.ExchangeName);
        foreach (var page in pages)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                logger.LogDebug(
                    $"Fetching prices for page:{_lastFetchedPage}/{pages.Count} from {request.ExchangeName}");
                var info = await exchangeInstance.FetchOHLCV(request.Symbol,
                    request.Timeframe.GetStringRepresentation(),
                    page.Since, Constants.PriceFetchLimit);
                // skipping items don't have timestamp returned as they will create conflict on database insert
                FetchedPrices.AddRange(
                    info
                        .Where(f => f.timestamp.HasValue)
                        .Select(s =>
                            new PriceDto(DateTime.UnixEpoch.AddMilliseconds(s.timestamp.GetValueOrDefault(0)),
                                (decimal)s.open.GetValueOrDefault(0),
                                (decimal)s.high.GetValueOrDefault(0),
                                (decimal)s.close.GetValueOrDefault(0),
                                (decimal)s.low.GetValueOrDefault(0),
                                (decimal)s.volume.GetValueOrDefault(0))
                        )
                );
                _lastFetchedPage++;
                logger.LogDebug($"Fetched prices >> #{info.Count}, {FetchedPrices.Last()}");
            }
            catch (Exception e)
            {
                return await OnError(e);
            }
        }

        logger.LogInformation($"Fetch operation finished for {request}");
        return await OnFinish();
    }

    public async Task<MethodResponse> OnFinish()
    {
        Guard.Against.Null(ActiveRequest);
        if (FetchedPrices.Count == 0)
        {
            return await OnError(new NoPriceFetchedException());
        }

        logger.LogDebug($"Fetched prices[{FetchedPrices.Count}] for {ActiveRequest}");

        // todo send fetched prices with mediatr
        logger.LogInformation($"Sending fetched price information to service. Last fetched page:{_lastFetchedPage}");
        // new PriceFetchCompletedCommand(priceInfo:, pluginId:, cacheKey:, tickerId:, timeFrame:);
        var result = await mediator.Send(new PriceFetchCompletedCommand(FetchedPrices,
            ActiveRequest.PluginId, ActiveRequest.CacheKey, ActiveRequest.TickerId, ActiveRequest.Timeframe));

        return await Task.FromResult(result);
    }

    public async Task<MethodResponse> OnError(Exception exception)
    {
        Guard.Against.Null(ActiveRequest);
        Guard.Against.Null(exception);
        // todo log the exception
        logger.LogError(exception, $"Failed to fetch prices for :{ActiveRequest}");
        await Task.Delay(1);
        throw exception;
    }
}