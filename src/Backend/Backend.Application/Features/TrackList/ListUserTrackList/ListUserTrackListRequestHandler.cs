using AutoMapper;
using Backend.Application.Abstraction.Repositories;
using Backend.Application.Abstraction.Services;
using Common.Application.Repositories;
using Common.Application.Services;
using Common.Core.DTOs.Backend;
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
            return dtos;
        var tickers = await tickerService.GetAvailableTickers();
        var items = await repository.GetUserTrackListAsync(request.UserId);

        foreach (var trackList in items)
        {
            var ticker = tickers.FirstOrDefault(f => f.Id == trackList.TickerId);
            dtos.Add(new TrackListDto(trackList.TickerId, trackList.UserId, ticker));
        }

        await cache.SetAsync<List<TrackListDto>>(cacheKey, dtos, TimeSpan.MaxValue);

        return dtos;
    }
}