using Common.Core.Models;


namespace Backend.Application.Abstraction.Services;

public interface IPluginService
{
    Task<List<PluginInfo>> GetAvailablePlugins();
    Task<PluginInfo> GetPluginInfo(string identifier);

    /// <summary>
    /// will connect to grpc and ask for free slot
    /// </summary>
    /// <returns></returns>
    Task<MethodResponse> CanRunNewPlugin();

    Task<MethodResponse> IsPluginInQueue(int pluginId);
}