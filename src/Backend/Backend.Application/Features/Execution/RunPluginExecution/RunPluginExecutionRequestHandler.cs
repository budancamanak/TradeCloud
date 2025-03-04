using Ardalis.GuardClauses;
using AutoMapper;
using Backend.Application.Abstraction.Repositories;
using Backend.Application.Abstraction.Services;
using Common.Application.Repositories;
using Common.Application.Services;
using Common.Core.Enums;
using Common.Core.Models;
using Common.Messaging.Abstraction;
using Common.Messaging.Events.PluginExecution;
using Common.Plugin.Models;
using Common.Web.Exceptions;
using FluentValidation;
using MediatR;

namespace Backend.Application.Features.Execution.RunPluginExecution;

public class RunPluginExecutionRequestHandler(
    IValidator<RunPluginExecutionRequest> validator,
    IPluginService pluginService,
    IEventBus messageBroker,
    ICacheService cache,
    IMapper mapper,
    IAnalysisExecutionRepository pluginRepository)
    : IRequestHandler<RunPluginExecutionRequest, MethodResponse>
{
    public async Task<MethodResponse> Handle(RunPluginExecutionRequest request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        var mr = await pluginService.CanRunNewPlugin();
        if (!mr.IsSuccess) return mr;
        mr = await pluginService.IsPluginInQueue(request.ExecutionId);
        if (mr.IsSuccess) return mr;
        var plugin = await pluginRepository.GetByIdAsync(request.ExecutionId);
        Guard.Against.Null(plugin);
        if (plugin.Status >= PluginStatus.Queued)
            throw new IllegalStateException("Plugin is already triggered");
        var paramModel = new PluginParamModel
        {
            ParamSet = plugin.ParamSet,
            TradingParams = plugin.TradingParams
        };
        await cache.SetAsync(CacheKeyGenerator.ActivePluginParamsKey(request.ExecutionId), paramModel,
            TimeSpan.MaxValue);
        var @event = mapper.Map<RunPluginRequestedEvent>(plugin);
        await messageBroker.PublishAsync(@event);
        // mr = await pluginRepository.SetPluginStatus(plugin.Id, PluginStatus.RunRequested);
        return mr;
    }
}