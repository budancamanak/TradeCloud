using Common.Core.Models;
using Common.Plugin.Abstraction;
using Worker.Application.Features.RunAnalysisRequested;

namespace Worker.Application.Abstraction;

public interface IPluginHost : IPluginStateManager
{
    IList<IPlugin> Plugins();
    bool AddAnalysisToQueue(RunAnalysisRequest requested);

    void RemovePluginFromQueue(int pluginId);

    // RunPluginRequest GetRequestFor(int pluginId);
    RunAnalysisRequest GetRequestFor(int pluginId);

    Task<MethodResponse> RunPlugin(int pluginId);

    // Tuple<IPlugin,string,string> GetPluginToRun(int requestExecutionId);
    Task<List<PluginRunInfo>> GetPluginToRun(int requestExecutionId);
    Task<MethodResponse> CanNewPluginRun();
    MethodResponse IsPluginInQueue(int pluginId);
}

public class PluginRunInfo
{
    public IPlugin Plugin { get; set; }
    public string PriceCacheKey { get; set; }
    public string TickerCacheKey { get; set; }
    public int PluginExecutionId { get; set; }
}