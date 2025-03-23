using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.Loader;
using Ardalis.GuardClauses;
using Common.Application.Repositories;
using Common.Application.Services;
using Common.Core.DTOs;
using Common.Core.Enums;
using Common.Core.Models;
using Common.Messaging.Events.AnalysisExecution;
using Common.Plugin.Abstraction;
using Common.Plugin.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Worker.Application.Abstraction;
using Worker.Application.Features.RunAnalysisRequested;
using Worker.Application.Features.RunPluginRequested;

namespace Worker.Infrastructure;

public class PluginHost : IPluginHost
{
    private static bool _pluginsLoaded = false;
    private readonly string _pluginFolder;
    private readonly ILogger<PluginHost> _logger;
    private readonly IPluginMessageBroker _messageBroker;
    private readonly int _maxActivePlugin;
    private readonly ICacheService _cache;
    private readonly ConcurrentDictionary<int, RunPluginRequest> _waitingPluginRequests;
    private readonly PluginLoader _pluginLoader;

    public PluginHost(IConfiguration configuration, IServiceScopeFactory scopeFactory,
        ICacheService cache, ILogger<PluginHost> logger)
    {
        _cache = cache;
        _waitingPluginRequests = new ConcurrentDictionary<int, RunPluginRequest>();
        using var scope = scopeFactory.CreateScope();
        _messageBroker = scope.ServiceProvider.GetRequiredService<IPluginMessageBroker>();
        _logger = logger;
        _pluginFolder = configuration["Plugins:Folder"] ??
                        $"{Path.GetDirectoryName(typeof(PluginHost).Assembly.Location)}";
        var max = configuration["Plugins:MaxConcurrentPluginRun"];
        _maxActivePlugin = string.IsNullOrWhiteSpace(max) ? 5 : int.Parse(max);
        if (_pluginsLoaded) return;
        _pluginLoader = new PluginLoader(scopeFactory, configuration, cache, _messageBroker, logger);
        _pluginsLoaded = true;
    }

    public IList<IPlugin> Plugins() => _pluginLoader.Plugins();

    public bool AddPluginToQueue(RunPluginRequest request)
    {
        return _waitingPluginRequests.TryAdd(request.ExecutionId, request);
    }

    public bool AddAnalysisToQueue(RunAnalysisRequest requested)
    {
        return true;
    }

    public RunPluginRequest GetRequestFor(int pluginId)
    {
        var item = _waitingPluginRequests.GetValueOrDefault(pluginId);
        if (item == null) throw new NotFoundException(pluginId.ToString(), "Plugin Request Not found ");
        return item;
    }

    public void RemovePluginFromQueue(int pluginId)
    {
        _waitingPluginRequests.TryRemove(pluginId, out _);
    }

    public async Task<MethodResponse> RunPlugin(int pluginId)
    {
        // plugin.Run(prices!);
        return MethodResponse.Success("Plugin ran");
    }

    public Tuple<IPlugin, string, string> GetPluginToRun(int pluginId)
    {
        _logger.LogInformation("PluginHost.RunPlugin requested: {}", pluginId);
        var request = _waitingPluginRequests.GetValueOrDefault(pluginId);
        if (request == null) throw new NotFoundException(pluginId.ToString(), "Plugin not found in waiting list");
        var cacheKey = CacheKeyGenerator.PluginKey(request.StartDate,
            request.EndDate, request.Ticker, request.Timeframe.TimeFrameFromString());
        var tickerCacheKey = CacheKeyGenerator.TickerKey(request.Ticker);
        // var prices = await _cache.GetAsync<List<PriceDto>>(cacheKey);
        // _logger.LogInformation("Got prices for plugin to run: {}", pluginId);
        // var ticker = await _cache.GetAsync<TickerDto>(CacheKeyGenerator.TickerKey(request.Ticker));
        // var paramModel =
        //     await _cache.GetAsync<PluginParamModel>(CacheKeyGenerator.ActivePluginParamsKey(request.ExecutionId));
        var plugin = _pluginLoader.CreatePlugin(request.Identifier);
        // _logger.LogInformation("Setting params for plugin to run: {} - {}", pluginId, paramModel?.ParamSet);
        // plugin.UseParamSet(paramModel?.ParamSet);
        // // plugin.UseLogger(_logger);
        // plugin.UseTicker(ticker!);
        // plugin.UseTradingParams(paramModel?.TradingParams);
        // plugin.UsePriceInfo(prices!);
        _logger.LogInformation("Executing plugin: {}", pluginId);
        // return new Tuple<IPlugin, TickerDto, List<PriceDto>>(plugin, ticker!, prices!);
        return new Tuple<IPlugin, string, string>(plugin, cacheKey, tickerCacheKey);
    }

    public MethodResponse IsPluginInQueue(int pluginId)
    {
        var request = _waitingPluginRequests.GetValueOrDefault(pluginId);
        return request == null
            ? MethodResponse.Error(pluginId, "Plugin is not in queue")
            : MethodResponse.Success(pluginId, "Plugin is in queue already");
    }

    public async Task<MethodResponse> CanNewPluginRun()
    {
        var activePlugins = await _cache.GetAsync<int>(CacheKeyGenerator.ActivePluginCountKey());
        return activePlugins == _maxActivePlugin
            ? MethodResponse.Error(activePlugins, "Too many active plugins exist")
            : MethodResponse.Success(0, "Plugin can run");
    }
}