using Common.Application.Repositories;
using Common.Application.Services;
using Common.Logging.Events.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Worker.Application.Abstraction;

namespace Worker.Application.Services;

public class WorkerCacheBuilder([FromKeyedServices("availablePlugins")] ICacheBuilder availablePluginsCacheBuilder)
    : ICacheBuilder
{
    public async Task BuildCacheAsync()
    {
        await availablePluginsCacheBuilder.BuildCacheAsync();
    }
}

public class AvailablePluginsCacheBuilder(
    IPluginHost pluginHost,
    ICacheService cache,
    ILogger<AvailablePluginsCacheBuilder> logger) : ICacheBuilder
{
    public async Task BuildCacheAsync()
    {
        logger.LogInformation(WorkerLogEvents.WorkerCache, "Building Available Plugins cache...");
        var plugins = pluginHost.Plugins();

        await CacheAction();
        return;

        async Task CacheAction()
        {
            foreach (var item in plugins)
            {
                logger.LogInformation(WorkerLogEvents.WorkerCache, "Adding plugin[{PluginInfo}] to cache",
                    item.GetPluginInfo());

                await cache.SetAsync(CacheKeyGenerator.AvailablePluginKey(item.GetPluginInfo().Identifier), item,
                    TimeSpan.MaxValue);
            }

            await cache.SetAsync("AvailablePlugins", plugins.Select(s => s.GetPluginInfo()).ToList(),
                TimeSpan.MaxValue);
        }
    }
}