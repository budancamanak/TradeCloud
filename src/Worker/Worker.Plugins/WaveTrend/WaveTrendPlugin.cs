using Common.Application.Repositories;
using Common.Core.Models;
using Common.Plugin.Abstraction;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
            ApSrc = 1,
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
        throw new NotImplementedException();
    }
}