﻿using AutoMapper;
using Backend.API.Models;
using Backend.Application.Features.Execution.AnalysisExecutionDetails;
using Backend.Application.Features.Execution.CreateAnalysisExecution;
using Backend.Application.Features.Execution.ListActivePlugins;
using Backend.Application.Features.Execution.ListAvailablePlugins;
using Backend.Application.Features.Execution.RunAnalysisExecution;
using Backend.Application.Features.Execution.StopAnalysisExecution;
using Backend.Application.Features.Execution.UserAnalysisExecutionList;
using Common.Application.Repositories;
using Common.Application.Services;
using Common.Core.DTOs.Backend;
using Common.Core.Enums;
using Common.Core.Models;
using Common.Security.Attributes;
using Common.Security.Enums;
using Common.Web.Http;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using StackExchange.Redis;

namespace Backend.API.Controllers;

// Level1: ViewerUser
// Level2: Level1+AnalyserUser
// Level3: Level2+SuperUser
// Level4: Admin
[ApiController]
[Route("[controller]")]
public class AnalysisExecutionsController(
    ILogger<AnalysisExecutionsController> logger,
    IHttpContextAccessor contextAccessor,
    ICacheService cache,
    IMediator mediator,
    IMapper mapper)
{
    [HttpGet("/AvailablePlugins")]
    [HasPermission(Permissions.Enum.RunAnalysis, Permissions.Enum.ManageScripts)]
    [HasRole(Roles.Enum.Admin, Roles.Enum.Analyst, Roles.Enum.ScriptDeveloper)]
    public async Task<List<PluginInfo>> GetAvailablePlugins()
    {
        var currentUser = contextAccessor.CurrentUser();
        var request = new ListAvailablePluginsRequest();
        var result = await mediator.Send(request);
        return result;
    }

    [HttpGet("/AvailablePluginParameters")]
    [HasPermission(Permissions.Enum.RunAnalysis, Permissions.Enum.ManageScripts)]
    [HasRole(Roles.Enum.Admin, Roles.Enum.Analyst, Roles.Enum.ScriptDeveloper)]
    public async Task<string> GetAvailablePluginPluginParameters(string identifier)
    {
        var currentUser = contextAccessor.CurrentUser();
        // var request = new ListAvailablePluginsRequest();
        // var result = await mediator.Send(request);
        // return result;
        var values = await cache.GetAsync<string>(CacheKeyGenerator.AvailablePluginParamsKey(identifier));
        return values;
    }


    [HttpGet("/ActivePlugins")]
    [HasPermission(Permissions.Enum.RunAnalysis, Permissions.Enum.ManageScripts)]
    [HasRole(Roles.Enum.Admin, Roles.Enum.Analyst, Roles.Enum.ScriptDeveloper)]
    public async Task<List<PluginExecutionsDto>> GetActivePlugins([FromQuery] ListActivePluginsRequest request)
    {
        var result = await mediator.Send(request);
        return result;
    }

    [HttpGet("User/{userId:int}/Info")]
    [HasPermission(Permissions.Enum.ViewResults)]
    [HasRole(Roles.Enum.Admin, Roles.Enum.Analyst, Roles.Enum.ScriptDeveloper, Roles.Enum.QA)]
    public async Task<List<UserAnalysisExecutionDto>> GetUserAnalysisExecutionInfos(int userId,
        [FromQuery] PluginStatus? status = null)
    {
        var request = new UserAnalysisExecutionListRequest(userId, status);
        var result = await mediator.Send(request);
        return result;
    }

    [HttpGet("{executionId:int}/Details")]
    [HasPermission(Permissions.Enum.ViewResults)]
    [HasRole(Roles.Enum.Admin, Roles.Enum.Analyst, Roles.Enum.ScriptDeveloper, Roles.Enum.QA)]
    public async Task<AnalysisExecutionDto> GetAnalysisExecutionDetails(int executionId, [FromQuery] int minimal = 0)
    {
        var request = new AnalysisExecutionDetailsRequest
            { AnalysisExecutionId = executionId, RequestMinimalInfo = minimal == 1 };
        var result = await mediator.Send(request);
        return result;
    }


    [HttpPost]
    [HasPermission(Permissions.Enum.RunAnalysis)]
    [HasRole(Roles.Enum.Admin, Roles.Enum.Analyst, Roles.Enum.ScriptDeveloper)]
    public async Task<MethodResponse> CreateAnalysisExecution([FromBody] CreateAnalysisExecutionModel model)
    {
        var request = mapper.Map<CreateAnalysisExecutionRequest>(model);
        var result = await mediator.Send(request);
        return result;
    }

    [HttpPatch]
    [HasPermission(Permissions.Enum.RunAnalysis)]
    [HasRole(Roles.Enum.Admin, Roles.Enum.Analyst, Roles.Enum.ScriptDeveloper)]
    public async Task<MethodResponse> RunAnalysisExecution([FromBody] RunAnalysisExecutionRequest request)
    {
        var result = await mediator.Send(request);
        return result;
    }

    [HttpDelete("{executionId:int}")]
    [HasPermission(Permissions.Enum.RunAnalysis)]
    [HasRole(Roles.Enum.Admin, Roles.Enum.Analyst, Roles.Enum.ScriptDeveloper)]
    public async Task<MethodResponse> StopAnalysisExecution(int executionId)
    {
        var request = new StopAnalysisExecutionRequest { AnalysisExecutionId = executionId };
        var result = await mediator.Send(request);
        return result;
    }
}