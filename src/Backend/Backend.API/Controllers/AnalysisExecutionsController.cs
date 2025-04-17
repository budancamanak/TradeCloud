using AutoMapper;
using Backend.API.Models;
using Backend.Application.Features.Execution.AnalysisExecutionDetails;
using Backend.Application.Features.Execution.CreateAnalysisExecution;
using Backend.Application.Features.Execution.ListActivePlugins;
using Backend.Application.Features.Execution.ListAvailablePlugins;
using Backend.Application.Features.Execution.RunAnalysisExecution;
using Backend.Application.Features.Execution.StopAnalysisExecution;
using Backend.Application.Features.Execution.UserAnalysisExecutionList;
using Common.Core.DTOs.Backend;
using Common.Core.Enums;
using Common.Core.Models;
using Common.Plugin.Abstraction;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AnalysisExecutionsController(
    ILogger<AnalysisExecutionsController> logger,
    IMediator mediator,
    IMapper mapper)
{
    [HttpGet("/AvailablePlugins")]
    public async Task<List<PluginInfo>> GetAvailablePlugins()
    {
        var request = new ListAvailablePluginsRequest();
        var result = await mediator.Send(request);
        return result;
    }

    [HttpGet("/ActivePlugins")]
    public async Task<List<PluginExecutionsDto>> GetActivePlugins([FromQuery] ListActivePluginsRequest request)
    {
        var result = await mediator.Send(request);
        return result;
    }

    [HttpGet("User/{userId:int}/Info")]
    public async Task<List<UserAnalysisExecutionDto>> GetUserAnalysisExecutionInfos(int userId,
        [FromQuery] PluginStatus? status = null)
    {
        var request = new UserAnalysisExecutionListRequest(userId, status);
        var result = await mediator.Send(request);
        return result;
    }

    [HttpGet("{executionId:int}/Details")]
    public async Task<AnalysisExecutionDto> GetAnalysisExecutionDetails(int executionId)
    {
        var request = new AnalysisExecutionDetailsRequest { AnalysisExecutionId = executionId };
        var result = await mediator.Send(request);
        return result;
    }

    [HttpPost]
    public async Task<MethodResponse> CreateAnalysisExecution([FromBody] CreateAnalysisExecutionModel model)
    {
        var request = mapper.Map<CreateAnalysisExecutionRequest>(model);
        var result = await mediator.Send(request);
        return result;
    }

    [HttpPatch]
    public async Task<MethodResponse> RunAnalysisExecution([FromBody] RunAnalysisExecutionRequest request)
    {
        var result = await mediator.Send(request);
        return result;
    }

    [HttpDelete("{executionId:int}")]
    public async Task<MethodResponse> StopAnalysisExecution(int executionId)
    {
        var request = new StopAnalysisExecutionRequest { AnalysisExecutionId = executionId };
        var result = await mediator.Send(request);
        return result;
    }
}