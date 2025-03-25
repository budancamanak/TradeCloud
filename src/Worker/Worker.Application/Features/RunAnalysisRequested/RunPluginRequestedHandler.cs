using Common.Core.Enums;
using Common.Core.Models;
using Common.Grpc;
using Common.Messaging.Abstraction;
using Common.Messaging.Events.PluginExecution;
using Google.Protobuf.WellKnownTypes;
using Hangfire;
using MediatR;
using Microsoft.Extensions.Logging;
using Worker.Application.Abstraction;

namespace Worker.Application.Features.RunAnalysisRequested;

public class RunAnalysisRequestedHandler(
    IPluginHost pluginHost,
    ILogger<RunAnalysisRequestedHandler> logger,
    IEventBus eventBus,
    IBackgroundJobClient jobClient,
    GrpcPriceController.GrpcPriceControllerClient client) : IRequestHandler<RunAnalysisRequest, MethodResponse>
{
    public async Task<MethodResponse> Handle(RunAnalysisRequest request, CancellationToken cancellationToken)
    {
        logger.LogWarning("RunPluginRequestedHandler called. Getting price info");
        var prices = await client.GetPricesForPluginAsync(new GrpcGetPricesRequest
        {
            Ticker = request.Ticker,
            Timeframe = request.Timeframe,
            EndDate = request.EndDate.ToTimestamp(),
            PluginId = request.ExecutionId,
            StartDate = request.StartDate.ToTimestamp()
        });
        if (prices == null || prices.Prices == null || prices.Prices.Count <= 0)
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
        foreach (var infoItem in info)
        {
            var identifier = jobClient.Enqueue(() => infoItem.Plugin.Run(infoItem.PluginExecutionId,
                infoItem.PriceCacheKey, infoItem.TickerCacheKey));
            // var identifier = jobClient.Enqueue(() => plugin.Item1.Run(request.ExecutionId, plugin.Item2, plugin.Item3));
            logger.LogDebug("Started background job to to run plugin[{}] : {}", request, identifier);
        }

        await eventBus.PublishAsync(new PluginStatusEvent(request.ExecutionId, PluginStatus.Queued));
        return MethodResponse.Success(request.ExecutionId, "Plugin started");
    }
}