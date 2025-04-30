using System.Linq.Expressions;
using Common.Core.Enums;
using Common.Core.Models;
using Common.Grpc;
using Common.Messaging.Abstraction;
using Common.Messaging.Events.PluginExecution;
using Common.Plugin.Abstraction;
using Google.Protobuf.WellKnownTypes;
using Hangfire;
using MediatR;
using Microsoft.Extensions.Logging;
using Worker.Application.Abstraction;

namespace Worker.Application.Features.RunAnalysisRequested;

public class RunAnalysisRequestedHandler(
    IPluginHost pluginHost,
    IPluginStateManager pluginStateManager,
    ILogger<RunAnalysisRequestedHandler> logger,
    IEventBus eventBus,
    IBackgroundJobClient jobClient,
    GrpcPriceService.GrpcPriceServiceClient client) : IRequestHandler<RunAnalysisRequest, MethodResponse>
{
    public async Task<MethodResponse> Handle(RunAnalysisRequest request, CancellationToken cancellationToken)
    {
        logger.LogWarning("RunAnalysisRequestedHandler called. Getting price info");
        var prices = await client.GetPricesForPluginAsync(new GrpcGetPricesRequest
        {
            Ticker = request.Ticker,
            Timeframe = request.Timeframe,
            EndDate = request.EndDate.ToTimestamp(),
            PluginId = request.ExecutionId,
            StartDate = request.StartDate.ToTimestamp()
        });
        if (prices?.Prices is not { Count: > 0 })
        {
            // todo send pluginupdated event over rabbitmq. change plugin state to waiting data
            await eventBus.PublishAsync(new PluginStatusEvent(request.ExecutionId, PluginStatus.WaitingData));
            pluginHost.AddAnalysisToQueue(request);
            return MethodResponse.Error(request.ExecutionId, "Plugin is in waiting queue now. Waiting for data");
        }

        // todo trigger hangfire !!
        // todo send pluginupdated event over rabbitmq. change plugin state to running
        pluginHost.AddAnalysisToQueue(request);
        var info = await pluginHost.GetPluginToRun(request.ExecutionId);
        pluginHost.RemovePluginFromQueue(request.ExecutionId);
        logger.LogDebug("Starting background job to to run plugin[{}]", request);

        string parent = "";
        foreach (var infoItem in info)
        {
            if (string.IsNullOrWhiteSpace(parent))
            {
                parent = jobClient.Enqueue(() =>
                    infoItem.Plugin.Run(request.ExecutionId, infoItem.PluginExecutionId, infoItem.PriceCacheKey,
                        infoItem.TickerCacheKey));
            }
            else
            {
                parent = jobClient.ContinueJobWith(parent,
                    () => infoItem.Plugin.Run(request.ExecutionId, infoItem.PluginExecutionId, infoItem.PriceCacheKey,
                        infoItem.TickerCacheKey));
            }
            
            await eventBus.PublishAsync(new PluginStatusEvent(infoItem.PluginExecutionId, PluginStatus.Queued));
            pluginStateManager.OnPluginStarted(infoItem.PluginExecutionId);
            logger.LogDebug("Started background job to to run plugin[{}] : {}", request, parent);
        }

        // await eventBus.PublishAsync(new PluginStatusEvent(request.ExecutionId, PluginStatus.Queued));
        return MethodResponse.Success(request.ExecutionId, "Plugin started");
    }
}