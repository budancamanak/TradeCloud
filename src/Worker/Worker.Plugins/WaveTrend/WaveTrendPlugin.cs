using Common.Application.Repositories;
using Common.Core.Models;
using Common.Plugin.Abstraction;
using Microsoft.Extensions.Logging;

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
        throw new NotImplementedException();
    }

    public override PluginInfo GetPluginInfo()
    {
        throw new NotImplementedException();
    }

    public override IParameters GetDefaultParamSet()
    {
        throw new NotImplementedException();
    }

    public override Type GetPluginType()
    {
        throw new NotImplementedException();
    }

    protected override void Execute()
    {
        throw new NotImplementedException();
    }
}