using Common.Core.Models;
using MediatR;

namespace Backend.Application.Features.Execution.ListAvailablePlugins;

public class ListAvailablePluginsRequest : IRequest<List<PluginInfo>>
{
}