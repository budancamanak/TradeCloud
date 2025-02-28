﻿using Backend.Application.Abstraction.Services;
using Common.Application.Repositories;
using Common.Application.Services;
using Common.Core.Models;
using Common.Grpc;
using Common.Plugin.Abstraction;

namespace Backend.Infrastructure.Services;

public class PluginService(
    ICacheService cache,
    GrpcAvailablePluginsController.GrpcAvailablePluginsControllerClient grpcClient) : IPluginService
{
    public async Task<List<IPlugin.PluginInfo>> GetAvailablePlugins()
    {
        var itemsOnCache = await cache.GetAsync<List<IPlugin.PluginInfo>>(CacheKeyGenerator.AvailablePlugins());
        if (itemsOnCache is { Count: > 0 })
            return itemsOnCache;
        var response = await grpcClient.GetAvailablePluginsAsync(new GrpcGetAvailablePluginsRequest());
        if (response == null) return itemsOnCache;
        itemsOnCache = new List<IPlugin.PluginInfo>();
        foreach (var info in response.Plugins)
        {
            itemsOnCache.Add(new IPlugin.PluginInfo(info.Name, info.Identifier, info.Version));
        }

        return itemsOnCache;
    }

    public async Task<IPlugin.PluginInfo> GetPluginInfo(string identifier)
    {
        var cached = await cache.GetAsync<IPlugin.PluginInfo>(CacheKeyGenerator.AvailablePluginKey(identifier));
        if (cached != null) return cached;
        var response = await grpcClient.GetAvailablePluginWithIdentifierAsync(new GrpcGetAvailablePluginWithIdentifier
        {
            Identifier = identifier
        });
        if (response != null)
        {
            cached = new IPlugin.PluginInfo(response.Name, response.Identifier, response.Version);
            await cache.SetAsync(CacheKeyGenerator.AvailablePluginKey(identifier), cached, TimeSpan.MaxValue);
        }

        return cached;
    }

    public async Task<MethodResponse> CanRunNewPlugin()
    {
        var mr = await grpcClient.GrpcCanRunNewPluginAsync(new GrpcCanRunNewPluginRequest());
        return mr.CanRun
            ? MethodResponse.Success("Can run new plugin")
            : MethodResponse.Error("There's no free slot to run the plugin");
    }

    public async Task<MethodResponse> IsPluginInQueue(int pluginId)
    {
        var mr = await grpcClient.GrpcIsPluginInQueueAsync(new GrpcIsPluginInQueueRequest { PluginId = pluginId });
        return mr.InQueue
            ? MethodResponse.Success("Plugin is already in queue")
            : MethodResponse.Error("Plugin is not in queue. Can be ran");
    }
}