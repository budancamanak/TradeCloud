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

namespace Backend.Application.Features.TrackList.RemoveUserTrackList;

public class RemoveUserTrackListRequestHandler(
    ITrackListRepository repository,
    IValidator<RemoveUserTrackListRequest> validator,
    IMapper mapper,
    ICacheService cache,
    ITickerService tickerService,
    ILogger<RemoveUserTrackListRequestHandler> logger) : IRequestHandler<RemoveUserTrackListRequest, MethodResponse>
{
    public async Task<MethodResponse> Handle(RemoveUserTrackListRequest request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        var ticker = await tickerService.GetTickerWithId(request.TickerId);
        if (ticker == null || string.IsNullOrWhiteSpace(ticker.Name))
        {
            logger.LogCritical(TrackListLogEvents.RemoveUserTrackList,
                "Failed to find ticker[{TickerId}] info to removed from User[{UserId}]'s Track List.",
                request.TickerId, request.UserId);
            throw new NotFoundException(nameof(request.TickerId), "Ticker");
        }

        var item = mapper.Map<Domain.Entities.TrackList>(request);
        var mr = await repository.DeleteAsync(item);
        if (mr.IsSuccess)
        {
            logger.LogInformation(TrackListLogEvents.RemoveUserTrackList,
                "Removed ticker[{TickerName}] from User[{UserId}]'s Track List. Updating cache..",
                ticker.Name, request.UserId);
            var cacheKey = CacheKeyGenerator.UserTrackingListKey(request.UserId);
            var userTracks =
                await cache.GetAsync<List<TrackListDto>>(cacheKey) ?? [];
            var dto = new TrackListDto(request.TickerId, request.UserId, ticker);
            var removed = userTracks.Remove(dto);
            if (removed)
                await cache.SetAsync(cacheKey, userTracks, TimeSpan.MaxValue);
            logger.LogInformation(TrackListLogEvents.RemoveUserTrackList,
                "Removed ticker[{TickerName}] from User[{UserId}]'s Track List. Updated cache..",
                ticker.Name, request.UserId);
        }
        else
        {
            logger.LogInformation(TrackListLogEvents.RemoveUserTrackList,
                "Failed to removed ticker[{TickerName}] from User[{UserId}]'s Track List. Reason: {Reason}",
                ticker.Name, request.UserId, mr.Message);
        }

        return mr;
    }
}