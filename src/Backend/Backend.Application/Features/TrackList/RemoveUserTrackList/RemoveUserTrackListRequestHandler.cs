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
            throw new NotFoundException(nameof(request.TickerId), "Ticker");
        var item = mapper.Map<Domain.Entities.TrackList>(request);
        var mr = await repository.DeleteAsync(item);
        if (mr.IsSuccess)
        {
            var cacheKey = CacheKeyGenerator.UserTrackingListKey(request.UserId);
            var userTracks =
                await cache.GetAsync<List<TrackListDto>>(cacheKey) ?? [];
            var dto = new TrackListDto(request.TickerId, request.UserId, ticker);
            var removed = userTracks.Remove(dto);
            if (removed)
                await cache.SetAsync(cacheKey, userTracks, TimeSpan.MaxValue);
        }

        return mr;
    }
}