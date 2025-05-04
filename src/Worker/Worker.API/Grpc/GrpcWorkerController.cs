using Ardalis.GuardClauses;
using AutoMapper;
using Common.Application.Repositories;
using Common.Grpc;
using Common.Logging.Events.Worker;
using Common.Plugin.Abstraction;
using Common.Security.Attributes;
using Common.Security.Enums;
using Grpc.Core;
using MediatR;
using Worker.Application.Abstraction;

namespace Worker.API.Grpc;

public class GrpcWorkerController(IPluginHost pluginHost, ICacheService cache, ILogger<GrpcWorkerController> logger)
    : GrpcAvailablePluginsService.GrpcAvailablePluginsServiceBase
{
    [HasPermission(Permissions.Enum.RunAnalysis, Permissions.Enum.ManageScripts)]
    [HasRole(Roles.Enum.Admin)]
    public override async Task<GrpcGetAvailablePluginsResponse> GetAvailablePlugins(
        GrpcGetAvailablePluginsRequest request,
        ServerCallContext context)
    {
        logger.LogInformation(WorkerLogEvents.GrpcWorkerAPI, "Getting available plugins");
        var plugins = pluginHost.Plugins().Select(f => f.GetPluginInfo()).ToList();

        var response = new GrpcGetAvailablePluginsResponse
        {
            Plugins =
            {
                plugins.Select(f => new GrpcAvailablePluginInfoResponse
                {
                    Identifier = f.Identifier,
                    Name = f.Name,
                    Version = f.Version
                }).ToList()
            }
        };
        return response;
    }

    [HasPermission(Permissions.Enum.RunAnalysis, Permissions.Enum.ManageScripts)]
    [HasRole(Roles.Enum.Admin)]
    public override async Task<GrpcAvailablePluginInfoResponse> GetAvailablePluginWithIdentifier(
        GrpcGetAvailablePluginWithIdentifierRequest request, ServerCallContext context)
    {
        var item = pluginHost.Plugins().FirstOrDefault(f => f.GetPluginInfo().Identifier == request.Identifier);
        if (item == null) throw new NotFoundException(request.Identifier, "Plugin not found");
        return await Task.FromResult(new GrpcAvailablePluginInfoResponse
        {
            Identifier = item.GetPluginInfo().Identifier,
            Name = item.GetPluginInfo().Name,
            Version = item.GetPluginInfo().Version
        });
    }

    [HasPermission(Permissions.Enum.RunAnalysis, Permissions.Enum.ManageScripts)]
    [HasRole(Roles.Enum.Admin)]
    public override async Task<GrpcCanRunNewPluginResponse> GrpcCanRunNewPlugin(GrpcCanRunNewPluginRequest request,
        ServerCallContext context)
    {
        var mr = await pluginHost.CanNewPluginRun();
        return new GrpcCanRunNewPluginResponse { CanRun = mr.IsSuccess };
    }

    [HasPermission(Permissions.Enum.RunAnalysis, Permissions.Enum.ManageScripts)]
    [HasRole(Roles.Enum.Admin)]
    public override Task<GrpcIsPluginInQueueResponse> GrpcIsPluginInQueue(GrpcIsPluginInQueueRequest request,
        ServerCallContext context)
    {
        var mr = pluginHost.IsPluginInQueue(request.PluginId);
        return Task.FromResult(new GrpcIsPluginInQueueResponse { InQueue = mr.IsSuccess });
    }
}