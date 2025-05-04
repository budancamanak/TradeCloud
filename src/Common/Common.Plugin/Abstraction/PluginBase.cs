using Common.Application.Repositories;
using Common.Application.Services;
using Common.Core.DTOs;
using Common.Core.Models;
using Common.Logging.Events.Worker;
using Common.Plugin.Math;
using Common.Plugin.Models;
using Microsoft.Extensions.Logging;


namespace Common.Plugin.Abstraction;

public abstract class PluginBase<T> : IPlugin where T : IParameters
{
    protected ILogger<IPlugin> Logger;
    protected IPluginMessageBroker MessageBroker;
    protected ICacheService Cache;

    protected TickerDto TickerDto;
    protected int ExecutionId;
    protected int AnalysisExecutionId;
    protected List<PriceDto> PriceInfo;

    // protected string? ParamsJson;
    protected string? _tradingParams;
    protected T Params;

    protected IPluginStateManager StateManager;
    protected TradeMath tradeMath;

    protected EventId LogEventId;

    public PluginBase(ILogger<IPlugin> logger, IPluginMessageBroker messageBroker, IPluginStateManager stateManager,
        ICacheService cache)
    {
        Cache = cache;
        Logger = logger;
        MessageBroker = messageBroker;
        StateManager = stateManager;
    }


    protected abstract T ParseParams(string? json);

    public abstract PluginInfo GetPluginInfo();

    public abstract IParameters GetDefaultParamSet();
    protected abstract void Execute();

    private void SetPluginParameters(string priceCacheKey, string tickerCacheKey, int analysisExecutionId, int pluginId)
    {
        ExecutionId = pluginId;
        AnalysisExecutionId = analysisExecutionId;
        var prices = Cache.GetAsync<List<PriceDto>>(priceCacheKey).GetAwaiter().GetResult();
        Logger.LogInformation(LogEventId, "Got prices for plugin to run: {PluginId}", pluginId);
        var ticker = Cache.GetAsync<TickerDto>(tickerCacheKey).GetAwaiter().GetResult();
        var paramModel =
            Cache.GetAsync<PluginParamModel>(CacheKeyGenerator.ActivePluginParamsKey(pluginId)).GetAwaiter()
                .GetResult();

        Logger.LogInformation(LogEventId, "Setting params for plugin to run: {PluginId} - {ParamSet}",
            pluginId, paramModel?.ParamSet);
        UseParamSet(paramModel?.ParamSet);
        // plugin.UseLogger(_logger);
        UseTicker(ticker!);
        UseTradingParams(paramModel?.TradingParams);
        UsePriceInfo(prices!);
    }

    public void Run(int analysisExecutionId, int pluginExecutionId, string priceCacheKey, string tickerCacheKey)
    {
        LogEventId = new EventId(999, GetPluginType().AssemblyQualifiedName);
        SetPluginParameters(priceCacheKey, tickerCacheKey, analysisExecutionId, pluginExecutionId);
        Logger.LogInformation(LogEventId, "Plugin[{PluginInfo}] started to run", GetPluginInfo());
        MessageBroker.OnPluginStarted(this, pluginExecutionId);
        try
        {
            Execute();
            MessageBroker.OnPluginSucceeded(this, pluginExecutionId);
            Logger.LogInformation("Plugin[{}] finished", GetPluginInfo());
        }
        catch (Exception ex)
        {
            MessageBroker.OnPluginFailed(this, pluginExecutionId, ex);
            Logger.LogCritical(LogEventId, "Plugin[{PluginInfo}] with failed: Exception:{Reason}",
                GetPluginInfo(), ex);
        }
    }

    public void UsePriceInfo(List<PriceDto> prices)
    {
        this.PriceInfo = prices;
        tradeMath = new TradeMath(Cache, AnalysisExecutionId, prices);
    }

    public void UseTicker(TickerDto tickerDto)
    {
        TickerDto = tickerDto;
    }

    public void UseLogger(ILogger<IPlugin> logger)
    {
        this.Logger = logger;
    }

    public void UseMessageBroker(IPluginMessageBroker messageBroker)
    {
        this.MessageBroker = messageBroker;
    }

    public void UseStateManager(IPluginStateManager stateManager)
    {
        this.StateManager = stateManager;
    }

    public void UseParamSet(string? paramsJson)
    {
        // this.ParamsJson = paramsJson;
        Params = ParseParams(paramsJson);
    }

    public void UseTradingParams(string? tradingParams)
    {
        _tradingParams = tradingParams;
    }

    public abstract Type GetPluginType();
}