using Common.Plugin.Signals;

namespace Common.Plugin.Abstraction;

public interface IPluginMessageBroker
{
    void OnPluginStarted(IPlugin plugin,int executionId);
    void OnPluginSucceeded(IPlugin plugin,int executionId);
    void OnPluginFailed(IPlugin plugin,int executionId, Exception exception);
    void OnPluginProgress(IPlugin plugin,int executionId, int current, int total);
    void OnPluginSignal(IPlugin plugin,int executionId, PluginSignal signal);
    void Log(string arg);
}