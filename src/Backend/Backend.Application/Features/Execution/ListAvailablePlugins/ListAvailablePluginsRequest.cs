using Common.Plugin.Abstraction;
using MediatR;

namespace Backend.Application.Features.Execution.ListAvailablePlugins;

public class ListAvailablePluginsRequest : IRequest<List<IPlugin.PluginInfo>>
{
}