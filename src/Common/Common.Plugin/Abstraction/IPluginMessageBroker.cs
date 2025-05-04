using Common.Plugin.Signals;

namespace Common.Plugin.Abstraction;

public interface IPluginMessageBroker
{
    Task OnPluginStarted(IPlugin plugin,int executionId);
    Task OnPluginSucceeded(IPlugin plugin,int executionId);
    Task OnPluginFailed(IPlugin plugin,int executionId, Exception exception);
    Task OnPluginProgress(IPlugin plugin,int executionId, int current, int total);
    Task OnAnalysisProgress(IPlugin plugin,int executionId, int increment, int total);
    Task OnPluginSignal(IPlugin plugin,int executionId, PluginSignal signal);
}