using Ardalis.GuardClauses;
using AutoMapper;
using Backend.Application.Abstraction.Repositories;
using Backend.Application.Abstraction.Services;
using Common.Application.Repositories;
using Common.Core.Enums;
using Common.Core.Models;
using Common.Logging.Events.Backend;
using Common.Messaging.Abstraction;
using Common.Messaging.Events.AnalysisExecution;
using Common.Web.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Features.Execution.RunAnalysisExecution;

public class RunAnalysisExecutionRequestHandler(
    IValidator<RunAnalysisExecutionRequest> validator,
    IPluginService pluginService,
    IEventBus messageBroker,
    ICacheService cache,
    IMapper mapper,
    ILogger<RunAnalysisExecutionRequestHandler> logger,
    IPluginExecutionEngine pluginExecutionEngine,
    IPluginExecutionRepository pluginRepository,
    IAnalysisExecutionRepository analysisRepository
)
    : IRequestHandler<RunAnalysisExecutionRequest, MethodResponse>
{
    public async Task<MethodResponse> Handle(RunAnalysisExecutionRequest request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        var mr = await pluginService.CanRunNewPlugin();
        if (!mr.IsSuccess)
        {
            logger.LogInformation(AnalysisExecutionLogEvents.RunAnalysisExecution,
                "Queue is full. Can't run {AnalysisExecution}", request.ExecutionId);
            return mr;
        }

        mr = await pluginService.IsPluginInQueue(request.ExecutionId);
        if (mr.IsSuccess)
        {
            logger.LogInformation(AnalysisExecutionLogEvents.RunAnalysisExecution,
                "Analysis[{AnalysisExecution}] is already in the queue", request.ExecutionId);
            return mr;
        }

        var plugin = await analysisRepository.GetByIdAsync(request.ExecutionId);
        Guard.Against.Null(plugin, message: $"Failed to find analysis execution with {request.ExecutionId}");
        // todo use grpc to ask worker about plugin status or use cache to store plugin status and fetch from there
        if (plugin.Status >= PluginStatus.Queued)
            throw new IllegalStateException("Plugin is already triggered");
        var executions = pluginExecutionEngine.GeneratePluginExecutions(plugin);
        int savedCount = 0, failedCount = 0;
        foreach (var item in executions)
        {
            try
            {
                mr = await pluginRepository.AddAsync(item);
                if (!mr.IsSuccess)
                {
                    logger.LogCritical(AnalysisExecutionLogEvents.RunAnalysisExecution,
                        "Failed to save plugin execution!! for : {PluginExecution}", item);
                    failedCount++;
                }
                else
                {
                    savedCount++;
                }
            }
            catch (AlreadySavedException e)
            {
                logger.LogDebug(AnalysisExecutionLogEvents.RunAnalysisExecution,
                    "Plugin[{PluginExecution}] is already saved. Safe exception skip. {Exception}", item, e);
                // pass.
                savedCount++;
            }
        }

        if (failedCount == executions.Count)
        {
            logger.LogCritical(AnalysisExecutionLogEvents.RunAnalysisExecution,
                "Failed to create plugin executions for analysis[{AnalysisExecution}]", request.ExecutionId);
            await analysisRepository.SetAnalysisExecutionProgress(request.ExecutionId, 1, 1);
            var failEvent = new AnalysisFinishedEvent();
            await messageBroker.PublishAsync(failEvent);
            return MethodResponse.Error(
                $"Failed to create plugin executions for analysis[{request.ExecutionId}]. Stopped execution");
        }

        executions = await pluginRepository.GetPluginExecutionsWithStatus(request.ExecutionId, PluginStatus.Init);
        var @event = mapper.Map<RunAnalysisRequestedEvent>(plugin,
            opts =>
            {
                opts.Items["PluginInfos"] = executions.Select(s => new RunPluginInfo
                {
                    PluginParameters = s.ParamSet,
                    PluginExecutionId = s.Id
                }).ToList();
            });
        logger.LogInformation(AnalysisExecutionLogEvents.RunAnalysisExecution,
            "Sending RunAnalysisRequestedEvent over message broker: {Event}", @event);
        await messageBroker.PublishAsync(@event);
        foreach (var execution in executions)
        {
            await pluginRepository.SetPluginStatus(execution.Id, PluginStatus.RunRequested);
        }

        logger.LogInformation(AnalysisExecutionLogEvents.RunAnalysisExecution,
            "Sent RunAnalysisRequestedEvent. Updated plugin executions' status: {Event}", @event);

        return mr;
    }
}