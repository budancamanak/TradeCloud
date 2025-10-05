using Common.Application.Repositories;
using Common.Core.Enums;
using Common.Core.Models;
using Common.Plugin.Abstraction;
using Common.Plugin.Math;
using Common.Plugin.Signals;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Skender.Stock.Indicators;

namespace Worker.Plugins.WaveTrend;

public class WaveTrendPlugin(
    ILogger<IPlugin> logger,
    IPluginMessageBroker messageBroker,
    IPluginStateManager stateManager,
    ICacheService cache)
    : PluginBase<WaveTrendPluginParams>(logger, messageBroker, stateManager, cache)
{
    protected override WaveTrendPluginParams ParseParams(string? json)
    {
        Logger.LogInformation(LogEventId, "Parsing params :{Params}", json);
        try
        {
            return !string.IsNullOrWhiteSpace(json)
                ? JsonConvert.DeserializeObject<WaveTrendPluginParams>(json)!
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
        return new PluginInfo("Wave Trend Indicator", "91d371bc-f92e-4861-b7b8-d7d86f6b874d", "1.0.0");
    }

    public override WaveTrendPluginParams GetDefaultParamSet()
    {
        return new WaveTrendPluginParams
        {
            ApSrc = nameof(CandlePart.HLC3),
            AverageLength = 21,
            ChannelLength = 10,
            CiMultiple = 1,
            OverBoughtLevel = 50,
            OverSoldLevel = -50,
            WaveTrend2Length = 4
        };
    }

    public override Type GetPluginType()
    {
        return typeof(WaveTrendPlugin);
    }

    protected override void Execute()
    {
        var quotes = PriceInfo.ToQuotes();
        // var _apSrc = "HL3";
        var _maType = "EMA";
        // var esa = quotes.CalculateMa(_maType, Params.ApSrc, Params.ChannelLength).ToList();
        var ap = quotes.Extract(Params.ApSrc);
        var esa = ap.ToTuple().CalculateMa(Params.ChannelLength);
        var diff = TradeMathEx.Diff(ap, esa, true).ToTuple();
        var d = diff.CalculateMa(Params.ChannelLength);
        var ciDiffUpper = TradeMathEx.Diff(ap, esa, false);
        var ciDiffLower = TradeMathEx.Multiply(d, Params.CiMultiple);
        var ci = TradeMathEx.Divide(ciDiffUpper, ciDiffLower).ToTuple();
        var wt1List = ci.CalculateMa(Params.AverageLength);
        var wt2List = wt1List.ToTuple().CalculateMa(Params.WaveTrend2Length);

        var prevLong = false;
        var prevShort = false;
        int rowId = 0;
        for (int i = 0; i < PriceInfo.Count; i++)
        {
            StateManager.ThrowIfCancelRequested(ExecutionId);
            var wt1 = wt1List.Find(PriceInfo[i].Timestamp);
            var wt2 = wt2List.Find(PriceInfo[i].Timestamp);
            MessageBroker.OnPluginProgress(this, ExecutionId, i + 1, PriceInfo.Count);
            if (wt1 is not { Ema: not null } || wt2 is not { Ema: not null })
            {
                Logger.LogDebug(LogEventId,
                    "Skipping {Index} due to null of WT values: WT1: {wt1}, WT2:{wt2}, price:{Price} @ {Date}", i,
                    wt1, wt2, PriceInfo[i].Close, PriceInfo[i].Timestamp);
                continue;
            }

            MessageBroker.OnPluginComputation(this, ExecutionId, rowId, "wt1.Ema", wt1.Ema.Value,
                PriceInfo[i].Timestamp);
            MessageBroker.OnPluginComputation(this, ExecutionId, rowId, "wt2.Ema", wt2.Ema.Value,
                PriceInfo[i].Timestamp);
            MessageBroker.OnPluginComputation(this, ExecutionId, rowId, "Params.OverBoughtLevel",
                Params.OverBoughtLevel, PriceInfo[i].Timestamp);

            bool goingDown = false, goingUp = false;
            if (wt1.Ema.Value > wt2.Ema.Value && wt2.Ema.Value - wt1.Ema.Value > 0 &&
                (wt2.Ema.Value + wt1.Ema.Value) / 2 >= Params.OverBoughtLevel)
            {
                goingDown = true;
            }

            if (wt1.Ema.Value > wt2.Ema.Value && wt2.Ema.Value - wt1.Ema.Value < 0 &&
                (wt2.Ema.Value + wt1.Ema.Value) / 2 <= Params.OverSoldLevel)
            {
                goingUp = true;
            }

            if (goingUp && !prevLong)
            {
                // turned bullish
                Logger.LogCritical(LogEventId, ">> We TURNED to bull. WT1: {wt1}, WT2:{wt2}, @ {Date}",
                    wt1, wt2, wt1?.Date);
                MessageBroker.OnPluginSignal(this, ExecutionId,
                    PluginSignal.CloseShort(TickerDto.Id, PriceInfo[i].Timestamp));
                MessageBroker.OnPluginSignal(this, ExecutionId,
                    PluginSignal.OpenLong(TickerDto.Id, PriceInfo[i].Timestamp));
                MessageBroker.OnPluginComputation(this, ExecutionId, rowId, "Output", (double)SignalType.OpenLong,
                    PriceInfo[i].Timestamp);
                prevLong = true;
                prevShort = false;
            }

            if (goingDown && !prevShort)
            {
                // turned bearish
                Logger.LogCritical(LogEventId, ">> We TURNED to bear. WT1: {wt1}, WT2:{wt2}, @ {Date}",
                    wt1, wt2, wt1?.Date);
                MessageBroker.OnPluginSignal(this, ExecutionId,
                    PluginSignal.CloseLong(TickerDto.Id, PriceInfo[i].Timestamp));
                MessageBroker.OnPluginSignal(this, ExecutionId,
                    PluginSignal.OpenShort(TickerDto.Id, PriceInfo[i].Timestamp));
                MessageBroker.OnPluginComputation(this, ExecutionId, rowId, "Output", (double)SignalType.OpenShort,
                    PriceInfo[i].Timestamp);
                prevShort = true;
                prevLong = false;
            }

            rowId++;
        }
    }
}