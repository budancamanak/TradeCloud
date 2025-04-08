using Common.Core.Enums;

namespace Common.Plugin.Abstraction;

public interface IPluginStateManager
{
    void ThrowIfCancelRequested(int pluginId);
    void OnPluginFinished(int pluginId);
    void OnPluginStarted(int pluginId);
    void OnPluginStopped(int pluginId);
}