using Ardalis.GuardClauses;
using AutoMapper;
using Backend.Application.Abstraction.Repositories;
using Backend.Application.Abstraction.Services;
using Common.Application.Repositories;
using Common.Application.Services;
using Common.Core.DTOs.Backend;
using Common.Core.Models;
using Common.Logging.Events.Backend;
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
        logger.LogInformation(TrackListLogEvents.AddTickerToTrackList,
            "Adding ticker[{Ticker}] to User[{UserId}]'s Track List", ticker.Name,
            request.UserId);
        var mr = await repository.AddAsync(item);
        if (mr.IsSuccess)
        {
            logger.LogInformation(TrackListLogEvents.AddTickerToTrackList,
                "Added ticker[{Ticker}] to User[{UserId}]'s Track List. Updating cache.",
                ticker.Name, request.UserId);
            var cacheKey = CacheKeyGenerator.UserTrackingListKey(request.UserId);
            var userTracks =
                await cache.GetAsync<List<TrackListDto>>(cacheKey) ?? [];
            userTracks.Add(new TrackListDto(request.TickerId, request.UserId, ticker));
            await cache.SetAsync(cacheKey, userTracks, TimeSpan.MaxValue);
            logger.LogInformation(TrackListLogEvents.AddTickerToTrackList,
                "Cache for User[{UserId}]'s Track List updated.", request.UserId);
        }

        return mr;
    }
}