using AutoMapper;
using Backend.API.Models;
using Backend.Application.Abstraction.Services;
using Backend.Application.Features.Execution.ListAvailablePlugins;
using Common.Core.Models;
using Common.Security.Attributes;
using Common.Security.Enums;
using Common.Web.Http;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
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

    [HttpPost("/Register")]
    [AllowAnon]
    public async Task<MethodResponse> RegisterUser([FromBody] RegisterUserModel request)
    {
        // var currentUser = contextAccessor.CurrentUser();
        // var request = new ListAvailablePluginsRequest();
        // var result = await mediator.Send(request);
        // return result;
        var mr = await userGrpcClient.RegisterUserAsync(request.Username, request.Email, request.Password,
            request.PasswordConfirm);
        return mr;
    }

    [HttpPost("/User/{userId:int}/Role/{roleId:int}")]
    [HasPermission(Permissions.Enum.AssignRoles, Permissions.Enum.ManageUsers)]
    [HasRole(Roles.Enum.Admin)]
    public async Task<MethodResponse> AddRoleToUser(int userId, int roleId)
    {
        // var currentUser = contextAccessor.CurrentUser();
        // var request = new ListAvailablePluginsRequest();
        // var result = await mediator.Send(request);
        // return result;
        var token = contextAccessor?.HttpContext?.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        if (string.IsNullOrWhiteSpace(token))
            return MethodResponse.Error("Unauthorized");
        var mr = await userGrpcClient.AddRoleToUserAsync(token, userId, roleId);
        return mr;
    }

    [HttpDelete("/User/{userId:int}/Role/{roleId:int}")]
    [HasPermission(Permissions.Enum.AssignRoles, Permissions.Enum.ManageUsers)]
    [HasRole(Roles.Enum.Admin)]
    public async Task<MethodResponse> RemoveRoleFromUser(int userId, int roleId)
    {
        // var currentUser = contextAccessor.CurrentUser();
        // var request = new ListAvailablePluginsRequest();
        // var result = await mediator.Send(request);
        // return result;
        var token = contextAccessor?.HttpContext?.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        if (string.IsNullOrWhiteSpace(token))
            return MethodResponse.Error("Unauthorized");
        var mr = await userGrpcClient.RemoveRoleFromUserAsync(token, userId, roleId);
        return mr;
    }


    [HttpPost("/Role/{roleId:int}/Permission/{permissionId:int}")]
    [HasPermission(Permissions.Enum.AssignRoles, Permissions.Enum.ManageUsers)]
    [HasRole(Roles.Enum.Admin)]
    public async Task<MethodResponse> AddPermissionToRole(int roleId, int permissionId)
    {
        // var currentUser = contextAccessor.CurrentUser();
        // var request = new ListAvailablePluginsRequest();
        // var result = await mediator.Send(request);
        // return result;
        var token = contextAccessor?.HttpContext?.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        if (string.IsNullOrWhiteSpace(token))
            return MethodResponse.Error("Unauthorized");
        var mr = await userGrpcClient.AddPermissionToRoleAsync(token, roleId, permissionId);
        return mr;
    }

    [HttpDelete("/Role/{roleId:int}/Permission/{permissionId:int}")]
    [HasPermission(Permissions.Enum.AssignRoles, Permissions.Enum.ManageUsers)]
    [HasRole(Roles.Enum.Admin)]
    public async Task<MethodResponse> RemovePermissionFromRole(int roleId, int permissionId)
    {
        // var currentUser = contextAccessor.CurrentUser();
        // var request = new ListAvailablePluginsRequest();
        // var result = await mediator.Send(request);
        // return result;
        var token = contextAccessor?.HttpContext?.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        if (string.IsNullOrWhiteSpace(token))
            return MethodResponse.Error("Unauthorized");
        var mr = await userGrpcClient.RemovePermissionFromRoleAsync(token, roleId, permissionId);
        return mr;
    }
}