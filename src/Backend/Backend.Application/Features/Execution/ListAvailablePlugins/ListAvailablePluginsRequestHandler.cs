﻿using Backend.Application.Abstraction.Services;
using Common.Plugin.Abstraction;
using FluentValidation;
using MediatR;

namespace Backend.Application.Features.Execution.ListAvailablePlugins;

public class ListAvailablePluginsRequestHandler(
    IValidator<ListAvailablePluginsRequest> validator,
    IPluginService pluginService)
    : IRequestHandler<ListAvailablePluginsRequest, List<IPlugin.PluginInfo>>
{
    public async Task<List<IPlugin.PluginInfo>> Handle(ListAvailablePluginsRequest request,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        // todo pluginServices does use cache & makes call to grpc to get available plugins
        // todo recheck this shit
        var items = await pluginService.GetAvailablePlugins();
        return items;
    }
}