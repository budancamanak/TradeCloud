using Ardalis.GuardClauses;
using Common.Core.DTOs;
using Common.Logging.Events.Market;
using FluentValidation;
using Hangfire;
using Market.Application.Abstraction.Services;
using Market.Application.Features.GetPricesForPlugin.Request;
using Market.Application.Services;
using Market.Application.Utilities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Market.Application.Features.GetPricesForPlugin;

public class GetPricesForPluginQueryHandler(
    IPriceService priceService,
    ITickerService tickerService,
    IPriceFetchCalculatorService fetchCalculatorService,
    IValidator<GetPricesForPluginQuery> validator,
    IPriceFetchJob fetchJob,
    IBackgroundJobClient jobClient,
    ILogger<GetPricesForPluginQueryHandler> logger)
    : IRequestHandler<GetPricesForPluginQuery, IList<PriceDto>>
{
    public async Task<IList<PriceDto>> Handle(GetPricesForPluginQuery request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        var fetchRequest = await tickerService.CreateFetchRequest(request);
        Guard.Against.Null(fetchRequest);
        var prices = await priceService.GetPricesForPluginAsync(request.CacheKey, request.PluginId,
            request.TickerId, request.Timeframe, request.StartDate, request.EndDate);
        if (!fetchCalculatorService.CheckPriceFetchIfNeeded(prices, request.StartDate, request.EndDate))
        {
            logger.LogInformation(MarketLogEvents.GetPricesForPluginQuery,
                "Price information is enough for plugin[{PluginId}] to run. cache key:{CacheKey}",
                request.PluginId, request.CacheKey);
            await priceService.CachePrices(prices, request.CacheKey);
            return prices;
        }

        var pages = PriceFetchPageCalculator.ToPages(request.Timeframe, request.StartDate, request.EndDate,
            Constants.PriceFetchLimit);

        logger.LogInformation(MarketLogEvents.GetPricesForPluginQuery,
            "Starting background job to fetch prices for plugin[{PluginId}], total pages:{TotalPages},Request: {Request}",
            request.PluginId, pages.Count, request);
        var identifier =
            jobClient.Enqueue(() => fetchJob.StartFetchPrices(pages, fetchRequest, CancellationToken.None));
        logger.LogInformation(MarketLogEvents.GetPricesForPluginQuery,
            "Started background job with id[{JobId}] to fetch prices for plugin[{PluginId}]", identifier,
            request.PluginId);
        return new List<PriceDto>();
    }
}