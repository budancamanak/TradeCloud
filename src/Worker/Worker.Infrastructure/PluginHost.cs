using System.Collections.Concurrent;
using Ardalis.GuardClauses;
using Common.Application.Repositories;
using Common.Application.Services;
using Common.Core.Enums;
using Common.Core.Models;
using Common.Plugin.Abstraction;
using Common.Plugin.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Worker.Application.Abstraction;
using Worker.Application.Features.RunAnalysisRequested;


namespace Worker.Infrastructure;

public class PluginHost : IPluginHost
{
    private static bool _pluginsLoaded = false;
    private readonly string _pluginFolder;
    private readonly ILogger<PluginHost> _logger;
    private readonly IPluginMessageBroker _messageBroker;
    private readonly int _maxActivePlugin;

    private readonly ICacheService _cache;

    // private readonly ConcurrentDictionary<int, RunPluginRequest> _waitingPluginRequests;
    private readonly ConcurrentDictionary<int, RunAnalysisRequest> _waitingAnalysisRequests;
    private readonly ConcurrentDictionary<int, PluginStatus> _activePlugins;

    private readonly PluginLoader _pluginLoader;

    public PluginHost(IConfiguration configuration, IServiceScopeFactory scopeFactory,
        ICacheService cache, ILogger<PluginHost> logger)
    {
        _cache = cache;
        // _waitingPluginRequests = new ConcurrentDictionary<int, RunPluginRequest>();
        _waitingAnalysisRequests = new ConcurrentDictionary<int, RunAnalysisRequest>();
        _activePlugins = new ConcurrentDictionary<int, PluginStatus>();
        using var scope = scopeFactory.CreateScope();
        _messageBroker = scope.ServiceProvider.GetRequiredService<IPluginMessageBroker>();
        _logger = logger;
        _pluginFolder = configuration["Plugins:Folder"] ??
                        $"{Path.GetDirectoryName(typeof(PluginHost).Assembly.Location)}";
        var max = configuration["Plugins:MaxConcurrentPluginRun"];
        _maxActivePlugin = string.IsNullOrWhiteSpace(max) ? 5 : int.Parse(max);
        if (_pluginsLoaded) return;
        logger.LogInformation("Loading plugins");
        _pluginLoader = new PluginLoader(scopeFactory, configuration, cache, _messageBroker, this, logger);
        _pluginsLoaded = true;
    }

    public IList<IPlugin> Plugins() => _pluginLoader.Plugins();

    public bool AddAnalysisToQueue(RunAnalysisRequest request)
    {
        return _waitingAnalysisRequests.TryAdd(request.ExecutionId, request);
    }

    // public RunPluginRequest GetRequestFor(int pluginId)
    // {
    //     var item = _waitingPluginRequests.GetValueOrDefault(pluginId);
    //     if (item == null) throw new NotFoundException(pluginId.ToString(), "Plugin Request Not found ");
    //     return item;
    // }

    public RunAnalysisRequest GetRequestFor(int pluginId)
    {
        var item = _waitingAnalysisRequests.GetValueOrDefault(pluginId);
        if (item == null) throw new NotFoundException(pluginId.ToString(), "Plugin Request Not found ");
        return item;
    }

    public void RemovePluginFromQueue(int pluginId)
    {
        _waitingAnalysisRequests.TryRemove(pluginId, out _);
    }

    public async Task<MethodResponse> RunPlugin(int pluginId)
    {
        // plugin.Run(prices!);
        return MethodResponse.Success("Plugin ran");
    }

    public async Task<List<PluginRunInfo>> GetPluginToRun(int pluginId)
    {
        _logger.LogInformation("PluginHost.RunPlugin requested: {}", pluginId);
        var items = new List<PluginRunInfo>();
        var request = _waitingAnalysisRequests.GetValueOrDefault(pluginId);
        if (request == null) throw new NotFoundException(pluginId.ToString(), "Plugin not found in waiting list");
        foreach (var info in request.PluginInfos)
        {
            var priceCacheKey = CacheKeyGenerator.PluginKey(request.StartDate,
                request.EndDate, request.Ticker, request.Timeframe.TimeFrameFromString());
            var tickerCacheKey = CacheKeyGenerator.TickerKey(request.Ticker);
            var plugin = _pluginLoader.CreatePlugin(request.Identifier);
            _logger.LogInformation("Executing plugin: {}", pluginId);
            await _cache.SetAsync(CacheKeyGenerator.ActivePluginParamsKey(info.PluginExecutionId),
                new PluginParamModel { TradingParams = "", ParamSet = info.PluginParameters }, TimeSpan.MaxValue);
            // return new Tuple<IPlugin, string, string>(plugin, cacheKey, tickerCacheKey);
            items.Add(new PluginRunInfo
            {
                Plugin = plugin,
                PriceCacheKey = priceCacheKey,
                TickerCacheKey = tickerCacheKey,
                PluginExecutionId = info.PluginExecutionId
            });
        }

        return items;
    }

    public MethodResponse IsPluginInQueue(int pluginId)
    {
        var request = _waitingAnalysisRequests.GetValueOrDefault(pluginId);
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

    public void ThrowIfCancelRequested(int pluginId)
    {
        if (!_activePlugins.TryGetValue(pluginId, out var status))
            throw new ArgumentException("Plugin not found in active list");
        _logger.LogInformation("Plugin[{}]'s status is {}", pluginId, status);
    }

    public void OnPluginFinished(int pluginId)
    {
        _activePlugins.TryRemove(pluginId, out _);
    }

    public void OnPluginStarted(int pluginId)
    {
        _activePlugins.TryAdd(pluginId, PluginStatus.Running);
    }

    public void OnPluginStopped(int pluginId)
    {
        _activePlugins.TryRemove(pluginId, out _);
    }
}