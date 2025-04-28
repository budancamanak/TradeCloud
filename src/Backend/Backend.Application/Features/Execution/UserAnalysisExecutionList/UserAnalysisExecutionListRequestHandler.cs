using AutoMapper;
using Backend.Application.Abstraction.Repositories;
using Backend.Application.Abstraction.Services;
using Backend.Domain.Entities;
using Common.Core.DTOs;
using Common.Core.DTOs.Backend;
using Common.Core.Enums;
using Common.Core.Models;
using FluentValidation;
using MediatR;

namespace Backend.Application.Features.Execution.UserAnalysisExecutionList;

public class UserAnalysisExecutionListRequestHandler(
    IValidator<UserAnalysisExecutionListRequest> validator,
    IAnalysisExecutionRepository analysisExecutionRepository,
    IPluginExecutionRepository pluginExecutionRepository,
    IPluginOutputRepository pluginOutputRepository,
    IMapper mapper,
    ITickerService tickerService,
    IPluginService pluginService) : IRequestHandler<UserAnalysisExecutionListRequest,
    List<UserAnalysisExecutionDto>>
{
    public async Task<List<UserAnalysisExecutionDto>> Handle(UserAnalysisExecutionListRequest request,
        CancellationToken cancellationToken)
    {
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
            Plugin = (plugins.FirstOrDefault(f => f.Identifier == analysis.PluginIdentifier) ??
                      PluginInfo.NULL_PLUGIN).Name,
            Ticker = (tickers.FirstOrDefault(f => f.Id == analysis.TickerId) ?? TickerDto.NULL_TICKER).Name,
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
}