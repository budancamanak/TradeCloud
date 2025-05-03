using AutoMapper;
using Common.Application.Repositories;
using Common.Application.Services;
using Common.Core.DTOs;
using Common.Logging.Events.Backend;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Features.TrackList.ListAvailableTickers;

public class ListAvailableTickersRequestHandler(
    IValidator<ListAvailableTickersRequest> validator,
    ICacheService cache,
    ILogger<ListAvailableTickersRequestHandler> logger,
    IMapper mapper)
    : IRequestHandler<ListAvailableTickersRequest, List<TickerDto>>
{
    public async Task<List<TickerDto>> Handle(ListAvailableTickersRequest request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        logger.LogInformation(TrackListLogEvents.ListAvailableTickers,"Handling ListAvailableTickersRequest");
        var key = CacheKeyGenerator.AvailableTickers();
        var tickers = await cache.GetAsync<List<TickerDto>>(key);
        logger.LogInformation(TrackListLogEvents.ListAvailableTickers, "Fetched tickers from cache. Ticker count: {}",
            tickers?.Count);
        // todo use grpc to fetch & update cache
        return tickers;
    }
}