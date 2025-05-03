using AutoMapper;
using Backend.Application.Abstraction.Repositories;
using Backend.Application.Abstraction.Services;
using Common.Application.Repositories;
using Common.Application.Services;
using Common.Core.DTOs.Backend;
using Common.Logging.Events.Backend;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Features.TrackList.ListUserTrackList;

public class ListUserTrackListRequestHandler(
    ITrackListRepository repository,
    IValidator<ListUserTrackListRequest> validator,
    IMapper mapper,
    ICacheService cache,
    ITickerService tickerService,
    ILogger<ListUserTrackListRequestHandler> logger)
    : IRequestHandler<ListUserTrackListRequest, List<TrackListDto>>
{
    public async Task<List<TrackListDto>> Handle(ListUserTrackListRequest request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        var cacheKey = CacheKeyGenerator.UserTrackingListKey(request.UserId);
        var dtos = await cache.GetAsync<List<TrackListDto>>(cacheKey) ?? [];
        if (dtos is { Count: > 0 })
        {
            logger.LogInformation(TrackListLogEvents.ListUserTrackList,
                "Fetched user track list from cache for User[{UserId}]'s. Count: {Count}", request.UserId, dtos.Count);
            return dtos;
        }

        logger.LogInformation(TrackListLogEvents.ListUserTrackList,
            "Fetching user track list from TickerService for User[{UserId}]'s.", request.UserId);

        var tickers = await tickerService.GetAvailableTickers();
        var items = await repository.GetUserTrackListAsync(request.UserId);

        foreach (var trackList in items)
        {
            var ticker = tickers.FirstOrDefault(f => f.Id == trackList.TickerId);
            if (ticker == null)
            {
                logger.LogWarning(TrackListLogEvents.ListUserTrackList,
                    "Unknown ticker with Id[{TickerId}] found in User[{UserId}]'s track list", trackList.TickerId,
                    request.UserId);
                continue;
            }

            dtos.Add(new TrackListDto(trackList.TickerId, trackList.UserId, ticker));
        }

        await cache.SetAsync(cacheKey, dtos, TimeSpan.MaxValue);
        logger.LogInformation(TrackListLogEvents.ListUserTrackList,
            "Fetched & cached user track list for User[{UserId}]'s. Count: {Count}", request.UserId, dtos.Count);

        return dtos;
    }
}