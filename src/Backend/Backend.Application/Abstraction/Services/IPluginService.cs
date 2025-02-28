using Common.Core.Models;
using Common.Plugin.Abstraction;

namespace Backend.Application.Abstraction.Services;

public interface IPluginService
{
    Task<List<IPlugin.PluginInfo>> GetAvailablePlugins();
    Task<IPlugin.PluginInfo> GetPluginInfo(string identifier);

    /// <summary>
    /// will connect to grpc and ask for free slot
    /// </summary>
    /// <returns></returns>
    Task<MethodResponse> CanRunNewPlugin();

    Task<MethodResponse> IsPluginInQueue(int pluginId);
}