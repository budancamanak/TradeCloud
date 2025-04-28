using AutoMapper;
using Common.Application.Repositories;
using Common.Grpc;
using Common.Plugin.Abstraction;
using Grpc.Core;
using MediatR;
using Worker.Application.Abstraction;

namespace Worker.API.Grpc;

public class WorkerGrpcController(IPluginHost pluginHost, ICacheService cache, ILogger<WorkerGrpcController> logger)
    : GrpcAvailablePluginsController.GrpcAvailablePluginsControllerBase
{
    public override async Task<GrpcGetAvailablePluginsResponse> GetAvailablePlugins(
        GrpcGetAvailablePluginsRequest request,
        ServerCallContext context)
    {
        logger.LogInformation("Getting available plugins");
        // var input = mapper.Map<GetPricesForPluginQuery>(request);
        // var data = await mediator.Send(input);
        // var output = mapper.Map<GrpcGetAvailablePluginsResponse>(data);
        // return output;
        // throw new NotImplementedException("");
        var plugins = pluginHost.Plugins().Select(f => f.GetPluginInfo()).ToList();

        var response = new GrpcGetAvailablePluginsResponse
        {
            Plugins =
            {
                plugins.Select(f => new GrpcAvailablePluginInfo
                {
                    Identifier = f.Identifier,
                    Name = f.Name,
                    Version = f.Version
                }).ToList()
            }
        };
        return response;
    }

    public override async Task<GrpcAvailablePluginInfo> GetAvailablePluginWithIdentifier(
        GrpcGetAvailablePluginWithIdentifier request, ServerCallContext context)
    {
        // var plugins = await cache.GetAsync<List<IPlugin.PluginInfo>>("AvailablePlugins");
        var item = pluginHost.Plugins().FirstOrDefault(f => f.GetPluginInfo().Identifier == request.Identifier);
        // var item = plugins.FirstOrDefault(f => f.Identifier == request.Identifier);
        return await Task.FromResult(new GrpcAvailablePluginInfo
        {
            Identifier = item.GetPluginInfo().Identifier,
            Name = item.GetPluginInfo().Name,
            Version = item.GetPluginInfo().Version
        });
    }

    public override async Task<GrpcCanRunNewPluginResponse> GrpcCanRunNewPlugin(GrpcCanRunNewPluginRequest request,
        ServerCallContext context)
    {
        var mr = await pluginHost.CanNewPluginRun();
        return new GrpcCanRunNewPluginResponse { CanRun = mr.IsSuccess };
    }

    public override Task<GrpcIsPluginInQueueResponse> GrpcIsPluginInQueue(GrpcIsPluginInQueueRequest request,
        ServerCallContext context)
    {
        var mr = pluginHost.IsPluginInQueue(request.PluginId);
        return Task.FromResult(new GrpcIsPluginInQueueResponse { InQueue = mr.IsSuccess });
    }
}