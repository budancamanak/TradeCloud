using Common.Application.Repositories;
using Common.Core.DTOs;
using Common.Core.Enums;
using Common.Core.Models;
using Common.Plugin.Abstraction;
using Common.Plugin.Math;
using Common.Plugin.Models;
using Common.Plugin.Signals;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Skender.Stock.Indicators;

namespace Worker.Plugins.FollowLineIndicator;

public class FollowLineIndicatorPlugin(
    ILogger<IPlugin> logger,
    IPluginMessageBroker messageBroker,
    IPluginStateManager stateManager,
    ICacheService cache)
    : PluginBase<FollowLineIndicatorPluginParams>(logger, messageBroker, stateManager, cache)
{
    protected override FollowLineIndicatorPluginParams ParseParams(string? json)
    {
        Logger.LogInformation(LogEventId, "Parsing params :{Params}", json);
        try
        {
            return !string.IsNullOrWhiteSpace(json)
                ? JsonConvert.DeserializeObject<FollowLineIndicatorPluginParams>(json)!
                : GetDefaultParamSet();
        }
        catch (Exception e)
        {
            Logger.LogError(LogEventId, "Exception happened when parsing plugin params: {Reason}", e.Message);
        }

        return GetDefaultParamSet();
    }

    public override PluginInfo GetPluginInfo()
    {
        return new PluginInfo("GoldenCrossDeathCross", "d0d004cc-6bb0-4b84-89f9-3677aae18ff9", "1.0.0");
    }

    public override FollowLineIndicatorPluginParams GetDefaultParamSet()
    {
        return new FollowLineIndicatorPluginParams
        {
            BBDeviation = 1,
            BBPeriod = 21,
            ATRPeriod = 5,
            UseATRFilter = 1
        };
    }

    public override Type GetPluginType()
    {
        return typeof(FollowLineIndicatorPlugin);
    }

    protected override void Execute()
    {
        Logger.LogWarning(LogEventId, "Plugin {PluginInfo} is running on {Ticker} with params: {Params}",
            GetPluginInfo(), TickerDto,
            Params.GetStringRepresentation());
        var bollinger = tradeMath.GetBollingerBands(Params.BBPeriod, Params.BBDeviation).Condense().ToList();
        var atrList = tradeMath.GetAtr(Params.ATRPeriod).ToList();
        var trendLine = new double[2];
        var iTrend = new int[2];
        for (var i = 0; i < PriceInfo.Count; i++)
        {
            trendLine[1] = trendLine[0];
            iTrend[1] = iTrend[0];
            var price = PriceInfo[i];
            StateManager.ThrowIfCancelRequested(ExecutionId);
            var bbResult = bollinger.Find(price.Timestamp);
            var bbUpper = bbResult?.UpperBand;
            var bbLower = bbResult?.LowerBand;
            var atr = atrList[i]?.Atr;
            MessageBroker.OnPluginProgress(this, ExecutionId, i + 1, PriceInfo.Count);
            if (!bbUpper.HasValue || !bbLower.HasValue || !atr.HasValue)
            {
                Logger.LogDebug(LogEventId,
                    "Skipping {Index} due to null of BB values: BBUpper: {BBUpper}, BBLower:{BBLower}, price:{Price} @ {Date}",
                    i, bbUpper, bbLower, PriceInfo[i].Close, PriceInfo[i].Timestamp);
                continue;
            }

            var bbSignal = 0;
            if (price.Close.ToDouble() > bbUpper.Value) bbSignal = 1;
            else if (price.Close.ToDouble() < bbLower) bbSignal = -1;

            if (bbSignal == 1 && Params.UseATRFilter == 1)
            {
                trendLine[0] = price.Low.ToDouble() - atr.Value;
                if (trendLine[0] < trendLine[1])
                    trendLine[0] = trendLine[1];
            }
            else if (bbSignal == -1 && Params.UseATRFilter == 1)
            {
                trendLine[0] = price.High.ToDouble() + atr.Value;
                if (trendLine[0] > trendLine[1] && trendLine[1] > 0)
                    trendLine[0] = trendLine[1];
            }
            else if (bbSignal == 0 && Params.UseATRFilter == 1)
            {
                trendLine[0] = trendLine[1];
            }
            else if (bbSignal == 1 && Params.UseATRFilter == 0)
            {
                trendLine[0] = price.Low.ToDouble();
                if (trendLine[0] < trendLine[1])
                    trendLine[0] = trendLine[1];
            }
            else if (bbSignal == -1 && Params.UseATRFilter == 0)
            {
                trendLine[0] = price.High.ToDouble();
                if (trendLine[0] > trendLine[1])
                    trendLine[0] = trendLine[1];
            }
            else if (bbSignal == 0 && Params.UseATRFilter == 0)
            {
                trendLine[0] = trendLine[1];
            }

            iTrend[0] = iTrend[1];
            if (trendLine[0] > trendLine[1])
                iTrend[0] = 1;
            else if (trendLine[0] < trendLine[1])
                iTrend[0] = -1;

            var buy = iTrend[1] == -1 && iTrend[0] == 1 ? 1 : 0;
            var sell = iTrend[1] == 1 && iTrend[0] == -1 ? 1 : 0;

            if (buy == 1)
            {
                Logger.LogCritical(LogEventId,
                    ">> We TURNED to BULL. trendLine[0]:{trendLine0}, trendLine[1]:{trendLine1}, iTrend[0]: {iTrend0}, iTrend[1]: {iTrend1} @ {Date}",
                    trendLine[0], trendLine[1], iTrend[0], iTrend[1], price.Timestamp);
                MessageBroker.OnPluginSignal(this, ExecutionId,
                    PluginSignal.CloseShort(TickerDto.Id, PriceInfo[i].Timestamp));
                MessageBroker.OnPluginSignal(this, ExecutionId,
                    PluginSignal.OpenLong(TickerDto.Id, PriceInfo[i].Timestamp));
            }
            else if (sell == 1)
            {
                Logger.LogCritical(LogEventId,
                    ">> We TURNED to BEAR. trendLine[0]:{trendLine0}, trendLine[1]:{trendLine1}, iTrend[0]: {iTrend0}, iTrend[1]: {iTrend1} @ {Date}",
                    trendLine[0], trendLine[1], iTrend[0], iTrend[1], price.Timestamp);
                MessageBroker.OnPluginSignal(this, ExecutionId,
                    PluginSignal.CloseLong(TickerDto.Id, PriceInfo[i].Timestamp));
                MessageBroker.OnPluginSignal(this, ExecutionId,
                    PluginSignal.OpenShort(TickerDto.Id, PriceInfo[i].Timestamp));
            }

            if (trendLine[1] == 0)
                trendLine[1] = trendLine[0];
        }
    }
}