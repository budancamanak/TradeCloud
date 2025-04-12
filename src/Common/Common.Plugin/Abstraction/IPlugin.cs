﻿using Common.Core.DTOs;
using Microsoft.Extensions.Logging;

namespace Common.Plugin.Abstraction;

public interface IPlugin
{
    // string Name();
    // string Identifier();

    PluginInfo GetPluginInfo();

    // void Run(TickerDto ticker,List<PriceDto> priceInfo,int executionId);
    void Run(int analysisExecutionId,int pluginExecutionId, string priceCacheKey, string tickerCacheKey);
    void UsePriceInfo(List<PriceDto> priceInfo);
    void UseTicker(TickerDto tickerDto);
    void UseLogger(ILogger<IPlugin> logger);
    void UseMessageBroker(IPluginMessageBroker messageBroker);
    void UseStateManager(IPluginStateManager stateManager);
    void UseParamSet(string? paramSet);
    void UseTradingParams(string? tradingParams);
    IParameters GetDefaultParamSet(); 
    public Type GetPluginType();

    record PluginInfo(string Name, string Identifier, string Version = "1.0.0")
    {
    }
}