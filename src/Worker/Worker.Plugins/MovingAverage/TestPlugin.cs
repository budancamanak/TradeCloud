using Common.Application.Repositories;
using Common.Core.DTOs;
using Common.Plugin.Abstraction;
using Microsoft.Extensions.Logging;

namespace Worker.Plugins.MovingAverage;

public class TestPlugin : PluginBase<GoldenDeathCrossPluginParamSet>
{
    public TestPlugin(ILogger<IPlugin> logger, IPluginMessageBroker messageBroker, ICacheService cache) : base(
        logger, messageBroker, cache)
    {
    }

    protected override GoldenDeathCrossPluginParamSet ParseParams(string? json)
    {
        return (GoldenDeathCrossPluginParamSet)GetDefaultParamSet();
    }

    public override IPlugin.PluginInfo GetPluginInfo()
    {
        return new IPlugin.PluginInfo("Test Plugin", "5cec4d93-f8e5-4741-bc18-782d760177fb");
    }

    public override IPluginParamSet GetDefaultParamSet()
    {
        return new GoldenDeathCrossPluginParamSet();
    }

    public override IPlugin NewInstance()
    {
        return new TestPlugin(this.Logger, this.MessageBroker, this.Cache);
    }

    protected override void Execute()
    {
        throw new NotImplementedException();
    }
}