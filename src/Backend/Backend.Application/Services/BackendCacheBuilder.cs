using AutoMapper;
using Backend.Application.Abstraction.Repositories;
using Common.Application.Repositories;
using Common.Application.Services;
using Common.Core.DTOs.Backend;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Services;

public class BackendCacheBuilder([FromKeyedServices("trackList")] ICacheBuilder trackListBuilder) : ICacheBuilder
{
    public async Task BuildCacheAsync()
    {
        await trackListBuilder.BuildCacheAsync();
    }
}

public class TrackListCacheBuilder(
    ICacheService cache,
    ITrackListRepository repository,
    IMapper mapper,
    ILogger<TrackListCacheBuilder> logger) : ICacheBuilder
{
    public async Task BuildCacheAsync()
    {
        logger.LogInformation("Building track list cache..");
        var users = await cache.GetAsync<List<int>>(CacheKeyGenerator.UserIdListKey()) ?? [];
        foreach (var id in users)
        {
            var trackList = await repository.GetUserTrackListAsync(id);
            var dtos = mapper.Map<List<TrackListDto>>(trackList);
            await cache.SetAsync(CacheKeyGenerator.UserTrackingListKey(id), dtos, TimeSpan.MaxValue);
        }
    }
}