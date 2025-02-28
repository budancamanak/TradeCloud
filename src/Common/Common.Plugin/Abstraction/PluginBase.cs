using Common.Application.Repositories;
using Common.Application.Services;
using Common.Core.DTOs;
using Common.Plugin.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Common.Plugin.Abstraction;

public abstract class PluginBase<T> : IPlugin where T : IPluginParamSet
{
    protected ILogger<IPlugin> Logger;
    protected IPluginMessageBroker MessageBroker;
    protected IReadOnlyCacheService Cache;

    protected TickerDto TickerDto;
    protected int ExecutionId;
    protected List<PriceDto> PriceInfo;

    // protected string? ParamsJson;
    protected string? _tradingParams;
    protected T Params;

    public PluginBase(ILogger<IPlugin> logger, IPluginMessageBroker messageBroker, IReadOnlyCacheService cache)
    {
        Cache = cache;
        Logger = logger;
        MessageBroker = messageBroker;
    }

    protected abstract void Execute();

    protected abstract T ParseParams(string? json);

    public abstract IPlugin.PluginInfo GetPluginInfo();

    public abstract IPluginParamSet GetDefaultParamSet();

    private void SetPluginParameters(string priceCacheKey, string tickerCacheKey, int pluginId)
    {
        ExecutionId = pluginId;
        var prices = Cache.GetAsync<List<PriceDto>>(priceCacheKey).GetAwaiter().GetResult();
        Logger.LogInformation("Got prices for plugin to run: {}", pluginId);
        var ticker = Cache.GetAsync<TickerDto>(tickerCacheKey).GetAwaiter().GetResult();
        var paramModel =
            Cache.GetAsync<PluginParamModel>(CacheKeyGenerator.ActivePluginParamsKey(pluginId)).GetAwaiter()
                .GetResult();

        Logger.LogInformation("Setting params for plugin to run: {} - {}", pluginId, paramModel?.ParamSet);
        UseParamSet(paramModel?.ParamSet);
        // plugin.UseLogger(_logger);
        UseTicker(ticker!);
        UseTradingParams(paramModel?.TradingParams);
        UsePriceInfo(prices!);
    }

    public void Run(int executionId, string priceCacheKey, string tickerCacheKey)
    {
        SetPluginParameters(priceCacheKey, tickerCacheKey, executionId);
        // TickerDto ticker, List<PriceDto> priceInfo,
        Logger.LogInformation("Plugin[{}] started to run", GetPluginInfo());
        MessageBroker.OnPluginStarted(this, executionId);
        try
        {
            Execute();
            MessageBroker.OnPluginSucceeded(this, executionId);
            Logger.LogInformation("Plugin[{}] finished", GetPluginInfo());
        }
        catch (Exception ex)
        {
            MessageBroker.OnPluginFailed(this, executionId, ex);
            Logger.LogError("Plugin[{}] with failed: Exception:{}", GetPluginInfo(), ex);
        }
    }

    public void UsePriceInfo(List<PriceDto> prices)
    {
        this.PriceInfo = prices;
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