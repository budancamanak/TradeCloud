using AutoMapper;
using Backend.API.Models;
using Backend.Application.Abstraction.Services;
using Backend.Application.Features.Execution.ListAvailablePlugins;
using Common.Core.Models;
using Common.Security.Attributes;
using Common.Web.Http;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(
    ILogger<AnalysisExecutionsController> logger,
    IHttpContextAccessor contextAccessor,
    // IMapper mapper,
    // IMediator mediator,
    IUserGrpcClient userGrpcClient)
{
    [HttpPost("/Login")]
    [AllowAnon]
    public async Task<MethodResponse> LoginUser([FromBody] UserLoginModel request)
    {
        // var currentUser = contextAccessor.CurrentUser();
        // var request = new ListAvailablePluginsRequest();
        // var result = await mediator.Send(request);
        // return result;
        var mr = await userGrpcClient.LoginUserAsync(request.Email, request.Password);
        return mr;
    }
}