using Common.Application.Repositories;
using Common.Plugin.Abstraction;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Worker.Plugins.Noop;

public class NoopPlugin : PluginBase<NoopPluginParams>
{
    public NoopPlugin(ILogger<IPlugin> logger, IPluginMessageBroker messageBroker,
        IPluginStateManager stateManager,
        ICacheService cache) : base(
        logger, messageBroker, stateManager, cache)
    {
    }


    protected override NoopPluginParams ParseParams(string? json)
    {
        return new NoopPluginParams();
    }

    public override IPlugin.PluginInfo GetPluginInfo()
    {
        return new IPlugin.PluginInfo("NoOpPlugin", "00000000-0000-0000-0000-000000000000", "1.0.0");
    }

    public override IParameters GetDefaultParamSet()
    {
        return new NoopPluginParams();
    }

    public override Type GetPluginType()
    {
        return typeof(NoopPlugin);
    }

    protected override void Execute()
    {
        Logger.LogWarning("Plugin {} is running on {} with params: {}", GetPluginInfo(), TickerDto,
            Params.GetStringRepresentation());
        var timeout = TimeSpan.FromSeconds(3);
        while (true)
        {
            StateManager.ThrowIfCancelRequested(ExecutionId);
            Logger.LogInformation("Plugin[{}] {} is running on {}", ExecutionId, GetPluginInfo(), TickerDto);
            Thread.Sleep(timeout);
        }
    }
}

public class NoopPluginParams : IParameters
{
    public IPluginParamSet GetParamSet()
    {
        return new NoopPluginParamSet();
    }

    public string GetStringRepresentation()
    {
        return "NoopPluginParams";
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
}

public class NoopPluginParamSet : IPluginParamSet
{
    public string GetStringRepresentation()
    {
        return "NoopPluginParamSet";
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
}