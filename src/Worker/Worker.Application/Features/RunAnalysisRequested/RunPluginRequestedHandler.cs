using System.Linq.Expressions;
using Common.Core.Enums;
using Common.Core.Models;
using Common.Grpc;
using Common.Logging.Events.Worker;
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
        logger.LogInformation(WorkerLogEvents.RunAnalysis,
            "Handling RunAnalysisRequest[{AnalysisExecution}]. Getting price info for request [{Request}]",
            request.ExecutionId, request);
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
            logger.LogWarning(WorkerLogEvents.RunAnalysis,
                "Not enough price info for RunAnalysisRequest[{AnalysisExecution}]. Waiting price info for request [{Request}]",
                request.ExecutionId, request);
            await eventBus.PublishAsync(new PluginStatusEvent(request.ExecutionId, PluginStatus.WaitingData));
            pluginHost.AddAnalysisToQueue(request);
            return MethodResponse.Error(request.ExecutionId, "Plugin is in waiting queue now. Waiting for data");
        }

        pluginHost.AddAnalysisToQueue(request);
        var info = await pluginHost.GetPluginToRun(request.ExecutionId);
        pluginHost.RemovePluginFromQueue(request.ExecutionId);
        logger.LogInformation("Starting background jobs to to run analysis[{AnalysisExecution}]", request.ExecutionId);

        string parent = "", current = "";
        foreach (var infoItem in info)
        {
            if (string.IsNullOrWhiteSpace(parent))
            {
                current = jobClient.Enqueue(() =>
                    infoItem.Plugin.Run(request.ExecutionId, infoItem.PluginExecutionId, infoItem.PriceCacheKey,
                        infoItem.TickerCacheKey));
            }
            else
            {
                current = jobClient.ContinueJobWith(parent,
                    () => infoItem.Plugin.Run(request.ExecutionId, infoItem.PluginExecutionId, infoItem.PriceCacheKey,
                        infoItem.TickerCacheKey));
            }

            await eventBus.PublishAsync(new PluginStatusEvent(infoItem.PluginExecutionId, PluginStatus.Queued));
            pluginStateManager.OnPluginStarted(infoItem.PluginExecutionId);
            logger.LogDebug("Started background job[{PluginIdentifier}] to to run plugin[{PluginId}] after {ParentJob}",
                current, infoItem.PluginExecutionId, parent);
            parent = current;
        }

        // await eventBus.PublishAsync(new PluginStatusEvent(request.ExecutionId, PluginStatus.Queued));
        return MethodResponse.Success(request.ExecutionId, "Plugin started");
    }
}