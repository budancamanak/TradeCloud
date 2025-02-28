using Common.Core.DTOs.Backend;
using Common.Plugin.Abstraction;
using MediatR;

namespace Backend.Application.Features.Execution.ListActivePlugins;

public class ListActivePluginsRequest : IRequest<List<PluginExecutionsDto>>
{
}