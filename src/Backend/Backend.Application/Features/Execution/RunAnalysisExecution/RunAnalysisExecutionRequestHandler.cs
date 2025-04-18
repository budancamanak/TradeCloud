﻿using Ardalis.GuardClauses;
using AutoMapper;
using Backend.Application.Abstraction.Repositories;
using Backend.Application.Abstraction.Services;
using Common.Application.Repositories;
using Common.Core.Enums;
using Common.Core.Models;
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
        /***
         * todo create plugin execution for each possible parameter set
         * todo save all plugin executions
         * todo trigger analysis run event.
         */
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        var mr = await pluginService.CanRunNewPlugin();
        if (!mr.IsSuccess) return mr;
        mr = await pluginService.IsPluginInQueue(request.ExecutionId);
        if (mr.IsSuccess) return mr;
        var plugin = await analysisRepository.GetByIdAsync(request.ExecutionId);
        Guard.Against.Null(plugin);
        // todo use grpc to ask worker about plugin status or use cache to store plugin status and fetch from there
        if (plugin.Status >= PluginStatus.Queued)
            throw new IllegalStateException("Plugin is already triggered");
        // todo generate param list here
        // todo foreach param list, save plugin execution
        // todo foreach execution, trigger runRequest
        var executions = pluginExecutionEngine.GeneratePluginExecutions(plugin);
        foreach (var item in executions)
        {
            try
            {
                mr = await pluginRepository.AddAsync(item);
                if (!mr.IsSuccess) logger.LogWarning("Failed to save plugin execution!! for : {}", item);
            }
            catch (AlreadySavedException e)
            {
                logger.LogDebug("Plugin is already saved. Safe exception skip. {}", e);
                // pass.
            }
        }

        // todo trigger rabbitmq to inform worker to run
        // todo use a method named GetPluginExecutionsToRun
        // todo this method will fetch executions to be run. states to fetch: init
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
        logger.LogWarning("Sending RunAnalysisRequestedEvent over message broker: {}", @event);
        await messageBroker.PublishAsync(@event);
        foreach (var execution in executions)
        {
            await pluginRepository.SetPluginStatus(execution.Id, PluginStatus.RunRequested);
        }

        return mr;
    }
}