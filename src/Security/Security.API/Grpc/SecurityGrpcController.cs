using AutoMapper;
using Common.Grpc;
using Common.Security.Enums;
using Common.Web.Http;
using Grpc.Core;
using MediatR;
using Security.Application.Abstraction.Repositories;
using Security.Application.Abstraction.Services;
using Security.Application.Features.User.RegisterUser;
using Security.Domain.Entities;
using Status = Common.Core.Enums.Status;

namespace Security.API.Grpc;

public class SecurityGrpcController(
    IUserService userService,
    ITokenService tokenService,
    IHttpContextAccessor contextAccessor,
    IUserRepository repository,
    IMapper mapper,
    IMediator mediator)
    : GrpcAuthController.GrpcAuthControllerBase
{
    public override async Task<CheckResponse> CheckPermission(CheckRequest request, ServerCallContext context)
    {
        var clientIp = context.RequestHeaders.GetValue("ClientIP") ?? "";
        var valid = await tokenService.ValidateToken(request.Token, clientIp);
        if (!valid.IsValid) return new CheckResponse { Granted = false };
        var userPermissions = await userService.GetUserPermissions(valid.UserId);
        var hasPermission = userPermissions.FirstOrDefault(f => f.Name == request.Value) != null;
        if (hasPermission)
            return new CheckResponse
            {
                Granted = true
            };
        return new CheckResponse
        {
            Granted = false
        };
    }

    public override Task<CheckResponse> CheckPolicy(CheckRequest request, ServerCallContext context)
    {
        return base.CheckPolicy(request, context);
    }

    public override async Task<CheckResponse> CheckRole(CheckRequest request, ServerCallContext context)
    {
        var clientIp = context.RequestHeaders.GetValue("ClientIP") ?? "";
        var valid = await tokenService.ValidateToken(request.Token, clientIp);
        if (!valid.IsValid) return new CheckResponse { Granted = false };
        var userRoles = await userService.GetUserRoles(valid.UserId);
        var hasPermission = userRoles.Any(f => f.Name == request.Value);
        if (hasPermission)
            return new CheckResponse
            {
                Granted = true
            };
        return new CheckResponse
        {
            Granted = false
        };
    }

    public override Task<CheckResponse> CheckScope(CheckRequest request, ServerCallContext context)
    {
        return base.CheckScope(request, context);
    }

    public override async Task<ValidateTokenResponse> ValidateToken(ValidateTokenRequest request,
        ServerCallContext context)
    {
        var ip = contextAccessor.GetClientIPv2();
        Console.WriteLine(context.Host);
        var clientIp = context.RequestHeaders.GetValue("ClientIP") ?? "";
        return await tokenService.ValidateToken(request.Token, clientIp);
    }

    public override async Task<UserLoginResponse> LoginUser(UserLoginRequest request, ServerCallContext context)
    {
        var clientIp = context.RequestHeaders.GetValue("ClientIP") ?? "";
        var mr = await userService.LoginUser(request.Email, request.Password, clientIp);
        return new UserLoginResponse
        {
            Message = mr.Message,
            Success = mr.IsSuccess,
            Token = mr.Data?.ToString()
        };
    }

    public override async Task<UserRegisterResponse> RegisterUser(UserRegisterRequest grpcRequest,
        ServerCallContext context)
    {
        var request = mapper.Map<RegisterUserRequest>(grpcRequest);
        var mr = await mediator.Send(request, context.CancellationToken);
        if (!mr.IsSuccess)
            return new UserRegisterResponse
            {
                Message = mr.Message,
                Success = mr.IsSuccess
            };
        return new UserRegisterResponse
        {
            Message = mr.Message,
            Success = mr.IsSuccess
        };
    }

    public override Task<UserInfoResponse> UserInfo(UserInfoRequest request, ServerCallContext context)
    {
        return base.UserInfo(request, context);
    }
}