using Backend.Application.Abstraction.Services;
using Common.Core.Models;
using Common.Logging.Events.Backend;
using Common.Plugin.Abstraction;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Features.Execution.ListAvailablePlugins;

public class ListAvailablePluginsRequestHandler(
    IValidator<ListAvailablePluginsRequest> validator,
    ILogger<ListAvailablePluginsRequestHandler> logger,
    IPluginService pluginService)
    : IRequestHandler<ListAvailablePluginsRequest, List<PluginInfo>>
{
    public async Task<List<PluginInfo>> Handle(ListAvailablePluginsRequest request,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        // todo pluginServices does use cache & makes call to grpc to get available plugins
        // todo recheck this shit
        logger.LogInformation(AnalysisExecutionLogEvents.ListAvailablePlugins, "Fetching available plugins");
        var items = await pluginService.GetAvailablePlugins();
        logger.LogInformation(AnalysisExecutionLogEvents.ListAvailablePlugins,
            "Fetching available plugins. Count: {Count}", items?.Count);
        return items;
    }
}