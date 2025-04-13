using Ardalis.GuardClauses;
using AutoMapper;
using Backend.Application.Abstraction.Repositories;
using Backend.Application.Abstraction.Services;
using Common.Core.DTOs.Backend;
using Common.Core.Enums;
using FluentValidation;
using MediatR;

namespace Backend.Application.Features.Execution.AnalysisExecutionDetails;

public class AnalysisExecutionDetailsRequestHandler(
    IValidator<AnalysisExecutionDetailsRequest> validator,
    IAnalysisExecutionRepository analysisExecutionRepository,
    IPluginExecutionRepository pluginExecutionRepository,
    IPluginOutputRepository pluginOutputRepository,
    IMapper mapper,
    IPluginService pluginService)
    : IRequestHandler<AnalysisExecutionDetailsRequest, AnalysisExecutionDto>
{
    public async Task<AnalysisExecutionDto> Handle(AnalysisExecutionDetailsRequest request,
        CancellationToken cancellationToken)
    {
        /***
         * todo get analysis execution
         * todo get plugin executions of that analysis
         * todo get outputs of each plugin executions
         * todo generate output and return
         *
         * todo do we need to use cache?
         *  todo to fetch plugin information we might need to access cache.
         */
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        var analysis = await analysisExecutionRepository.GetByIdAsync(request.AnalysisExecutionId);
        Guard.Against.Null(analysis);
        var pluginInfo = await pluginService.GetPluginInfo(analysis.PluginIdentifier);
        Guard.Against.Null(pluginInfo);

        var result = new AnalysisExecutionDto
        {
            Id = analysis.Id,
            PluginInfo = pluginInfo,
            Status = analysis.Status.GetStringRepresentation(),
            EndDate = analysis.EndDate,
            StartDate = analysis.StartDate
        };

        var plugins = await pluginExecutionRepository.GetPluginOfAnalysis(request.AnalysisExecutionId);
        result.PluginExecutions = mapper.Map<List<PluginExecutionsDto>>(plugins).ToArray();
        foreach (var item in result.PluginExecutions)
        {
            var outputs = await pluginOutputRepository.GetPluginOutputs(item.Id);
            var outputDtos =
                mapper.Map<List<PluginOutputDto>>(outputs, opts => { opts.Items["PluginName"] = pluginInfo.Name; });
            item.Outputs = outputDtos.ToArray();
        }

        if (plugins.Count == 0) result.Status = PluginStatus.Init.GetStringRepresentation();
        else if (plugins.All(f => f.Status == PluginStatus.Init))
            result.Status = PluginStatus.Init.GetStringRepresentation();
        else if (plugins.All(f => f.Status == PluginStatus.Success))
            result.Status = PluginStatus.Success.GetStringRepresentation();
        else if (plugins.All(f => f.Status == PluginStatus.Failure))
            result.Status = PluginStatus.Failure.GetStringRepresentation();
        else result.Status = PluginStatus.Running.GetStringRepresentation();

        return result;
    }
}