using AutoMapper;
using Backend.Application.Abstraction.Repositories;
using Backend.Application.Abstraction.Services;
using Backend.Domain.Entities;
using Common.Core.DTOs;
using Common.Core.DTOs.Backend;
using Common.Core.Enums;
using Common.Core.Models;
using Common.Logging.Events.Backend;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Features.Execution.UserAnalysisExecutionList;

public class UserAnalysisExecutionListRequestHandler(
    IValidator<UserAnalysisExecutionListRequest> validator,
    IAnalysisExecutionRepository analysisExecutionRepository,
    IPluginExecutionRepository pluginExecutionRepository,
    IPluginOutputRepository pluginOutputRepository,
    IMapper mapper,
    ILogger<UserAnalysisExecutionListRequestHandler> logger,
    ITickerService tickerService,
    IPluginService pluginService) : IRequestHandler<UserAnalysisExecutionListRequest,
    List<UserAnalysisExecutionDto>>
{
    public async Task<List<UserAnalysisExecutionDto>> Handle(UserAnalysisExecutionListRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogDebug(AnalysisExecutionLogEvents.UserAnalysisExecutionList,
            "Validating user[{UserId}] analysis execution list request", request.UserId);
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        var plugins = await pluginService.GetAvailablePlugins();
        var tickers = await tickerService.GetAvailableTickers();
        List<AnalysisExecution> analysisList;
        if (request.Status.HasValue)
            analysisList =
                await analysisExecutionRepository.GetUserAnalysisExecutions(request.UserId, request.Status.Value);
        else
            analysisList =
                await analysisExecutionRepository.GetUserAnalysisExecutions(request.UserId);
        var output = analysisList.Select(analysis => new UserAnalysisExecutionDto
        {
            Id = analysis.Id,
            Plugin = GetPluginInfo(plugins, analysis).Name,
            Ticker = GetTickerInfo(tickers, analysis).Name,
            Status = analysis.Status.GetStringRepresentation(),
            EndDate = analysis.EndDate,
            StartDate = analysis.StartDate,
            Timeframe = analysis.Timeframe.GetStringRepresentation(),
            ParamSet = analysis.ParamSet,
            Progress = analysis.Progress,
            TradingParams = analysis.TradingParams,
            PluginExecutions = mapper.Map<List<PluginExecutionsDto>>(analysis.PluginExecutions).ToArray()
        }).ToList();
        return output;
    }

    private TickerDto GetTickerInfo(List<TickerDto> tickers, AnalysisExecution analysis)
    {
        var ticker = tickers.FirstOrDefault(f => f.Id == analysis.TickerId);
        if (ticker != null) return ticker;
        logger.LogCritical(AnalysisExecutionLogEvents.UserAnalysisExecutionList,
            "Failed to find ticker info with [{Ticker}] for analysis[{AnalysisExecution}]",
            analysis.TickerId, analysis.Id);
        return TickerDto.NULL_TICKER;
    }


    private PluginInfo GetPluginInfo(List<PluginInfo> plugins, AnalysisExecution analysis)
    {
        var plugin = plugins.FirstOrDefault(f => f.Identifier == analysis.PluginIdentifier);
        if (plugin != null) return plugin;
        logger.LogCritical(AnalysisExecutionLogEvents.UserAnalysisExecutionList,
            "Failed to find plugin info with [{PluginIdentifier}] for analysis[{AnalysisExecution}]",
            analysis.PluginIdentifier, analysis.Id);
        return PluginInfo.NULL_PLUGIN;
    }
}