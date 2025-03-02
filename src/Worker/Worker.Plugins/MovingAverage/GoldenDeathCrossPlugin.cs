using Common.Application.Repositories;
using Common.Plugin.Abstraction;
using Common.Plugin.Signals;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Skender.Stock.Indicators;
using Worker.Plugins.Utils;

namespace Worker.Plugins.MovingAverage;

public class GoldenDeathCrossPlugin : PluginBase<GoldenDeathCrossPluginParamSet>
{
    public GoldenDeathCrossPlugin(ILogger<IPlugin> logger, IPluginMessageBroker messageBroker,
        IReadOnlyCacheService cache) : base(logger, messageBroker, cache)
    {
    }

    protected override GoldenDeathCrossPluginParamSet ParseParams(string? json)
    {
        Logger.LogWarning("Parsing params :{}", json);
        try
        {
            return !string.IsNullOrWhiteSpace(json)
                ? JsonConvert.DeserializeObject<GoldenDeathCrossPluginParamSet>(json)!
                : GetDefaultParamSet();
        }
        catch (Exception e)
        {
            Logger.LogError("Exception happened when parsing plugin params: {}", e.Message);
        }

        return GetDefaultParamSet();
    }

    public override IPlugin.PluginInfo GetPluginInfo()
    {
        return new IPlugin.PluginInfo("GoldenCrossDeathCross", "09a0a20a-666c-4b13-80f7-5dc04db19f8c", "1.0.1");
    }

    public override GoldenDeathCrossPluginParamSet GetDefaultParamSet()
    {
        return new GoldenDeathCrossPluginParamSet
        {
            FastMovingAvg = 50,
            SlowMovingAvg = 200
        };
    }

    public override Type GetPluginType()
    {
        return typeof(GoldenDeathCrossPlugin);
    }

    // todo enable elk stack.
    // todo think of param set range -> purpose of this project
    // todo create plugin for each param set range? -> must have parent/child plugin architecture
    // todo execute plugin for each param set range? -> must update progress & signal workflows.
    // todo continuous plugin run -> fetch prices[based on fetch config-(fetch window etc)]
    // todo run plugin when fetch done. -> for selected param set at first .
    // todo think of pnl analysis. can make use of trading params.
    // todo trading params can have: tp & sl percentages, close on other side signal. -> can be used for pnl analysis
    // todo develop trading micro service for above. trading micro service can have MockBroker
    // todo MockBroker will simulate trading operations & pnl analysis. might need to fetch all prices between signals.
    // todo think & implement continuous price fetch -> for pnl analysis & continuous plugin run 
    // todo make use of jenkins -> to fetch & run tests.
    // todo allow users to upload their codes. use sandboxing & compiling
    // todo write worker.tests
    // todo write integration tests(api tests)

    protected override void Execute()
    {
        Logger.LogWarning("Plugin {} is running on {} with params: {}", GetPluginInfo(), TickerDto,
            Params.GetStringRepresentation());
        Thread.Sleep(5000);
        var quotes = PriceInfo.ToQuotes();

        var slow = quotes.GetSma(Params.SlowMovingAvg).ToList();
        var fast = quotes.GetSma(Params.FastMovingAvg).ToList();
        var isLastLong = 0;
        for (var i = 0; i < PriceInfo.Count; i++)
        {
            var slowResult = slow.Find(PriceInfo[i].Timestamp);
            var slowSma = slowResult?.Sma;
            var fastSma = fast.Find(PriceInfo[i].Timestamp)?.Sma;
            MessageBroker.OnPluginProgress(this, ExecutionId, i + 1, PriceInfo.Count);
            if (!slowSma.HasValue || !fastSma.HasValue)
            {
                Logger.LogInformation("Skipping {} due to null of sma values: slow: {}, fast:{}, price:{} @ {}", i,
                    slowSma,
                    fastSma, PriceInfo[i].Close, PriceInfo[i].Timestamp);
                continue;
            }

            bool currentLong = false;
            if (fastSma.Value > slowSma.Value)
            {
                // go long
                currentLong = true;
                Logger.LogInformation(">> We are in bull. fast:{}, slow: {}, timestamp: {}, close: {}", fastSma.Value,
                    slowSma.Value, PriceInfo[i].Timestamp, PriceInfo[i].Close);
            }
            else if (fastSma.Value < slowSma.Value)
            {
                // go short
                currentLong = false;
                Logger.LogInformation(">> We are in bear. fast:{}, slow: {}, timestamp: {}, close: {}", fastSma.Value,
                    slowSma.Value, PriceInfo[i].Timestamp, PriceInfo[i].Close);
            }

            if (isLastLong == 0)
            {
                if (currentLong) isLastLong = -1;
                else isLastLong = 1;
            }


            if (isLastLong == 1 && !currentLong)
            {
                // turned bearish
                Logger.LogWarning(">> We TURNED to bear. fast:{}, slow: {} @ {}", fastSma.Value, slowSma.Value,
                    slowResult?.Date);
                MessageBroker.OnPluginSignal(this, ExecutionId,
                    PluginSignal.CloseLong(TickerDto.Id, PriceInfo[i].Timestamp));
                MessageBroker.OnPluginSignal(this, ExecutionId,
                    PluginSignal.OpenShort(TickerDto.Id, PriceInfo[i].Timestamp));
            }
            else if (isLastLong == -1 && currentLong)
            {
                // turned bullish
                Logger.LogWarning(">> We TURNED to bull. fast:{}, slow: {}", fastSma.Value, slowSma.Value);
                MessageBroker.OnPluginSignal(this, ExecutionId,
                    PluginSignal.CloseShort(TickerDto.Id, PriceInfo[i].Timestamp));
                MessageBroker.OnPluginSignal(this, ExecutionId,
                    PluginSignal.OpenLong(TickerDto.Id, PriceInfo[i].Timestamp));
            }

            isLastLong = currentLong ? 1 : -1;


            // _logger.LogInformation(">> Price[{}]: {}", i, priceInfo[i].Close);
            // _messageBroker.OnPluginProgress(this, i, 10);
            // if (i % 3 == 0)
            //     _messageBroker.OnPluginSignal(this, PluginSignal.OpenLong(ticker.Id));
        }
    }
}