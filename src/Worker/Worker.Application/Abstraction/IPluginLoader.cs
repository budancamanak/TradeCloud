using Common.Plugin.Abstraction;

namespace Worker.Application.Abstraction;

public interface IPluginLoader
{
    IList<IPlugin> LoadPlugins();
}