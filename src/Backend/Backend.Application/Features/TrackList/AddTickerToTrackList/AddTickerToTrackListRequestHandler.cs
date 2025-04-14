using Ardalis.GuardClauses;
using AutoMapper;
using Backend.Application.Abstraction.Repositories;
using Backend.Application.Abstraction.Services;
using Common.Application.Repositories;
using Common.Application.Services;
using Common.Core.DTOs.Backend;
using Common.Core.Models;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Features.TrackList.AddTickerToTrackList;

public class AddTickerToTrackListRequestHandler(
    ITrackListRepository repository,
    IValidator<AddTickerToTrackListRequest> validator,
    IMapper mapper,
    ICacheService cache,
    ITickerService tickerService,
    ILogger<AddTickerToTrackListRequestHandler> logger)
    : IRequestHandler<AddTickerToTrackListRequest, MethodResponse>
{
    public async Task<MethodResponse> Handle(AddTickerToTrackListRequest request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        var ticker = await tickerService.GetTickerWithId(request.TickerId);
        if (ticker == null || string.IsNullOrWhiteSpace(ticker.Name))
            throw new NotFoundException(nameof(request.TickerId), "Ticker");
        var item = mapper.Map<Domain.Entities.TrackList>(request);
        var mr = await repository.AddAsync(item);
        if (mr.IsSuccess)
        {
            var cacheKey = CacheKeyGenerator.UserTrackingListKey(request.UserId);
            var userTracks =
                await cache.GetAsync<List<TrackListDto>>(cacheKey) ?? [];
            userTracks.Add(new TrackListDto(request.TickerId, request.UserId, ticker));
            await cache.SetAsync(cacheKey, userTracks, TimeSpan.MaxValue);
        }

        return mr;
    }
}