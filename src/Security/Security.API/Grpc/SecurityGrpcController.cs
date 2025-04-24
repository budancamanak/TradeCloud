using Common.Grpc;
using Grpc.Core;
using Security.Application.Abstraction.Services;
using Security.Domain.Entities;
using Status = Common.Core.Enums.Status;

namespace Security.API.Grpc;

public class SecurityGrpcController(IUserService userService) : GrpcAuthController.GrpcAuthControllerBase
{
    public override async Task<CheckResponse> CheckPermission(CheckRequest request, ServerCallContext context)
    {
        var userRoles = await userService.GetUserRoles(request.Token);
        var hasPermission = userRoles.Any(f => f.Permissions.FirstOrDefault(fx => fx.Name == request.Value) != null);
        if (hasPermission)
            return new CheckResponse
            {
                Granted = true
            };
        // var permissions = await userService.GetUserPermissions(request.Token);
        // hasPermission = permissions.Any(f => f.Name == request.Value);
        // if (hasPermission)
        //     return new CheckResponse
        //     {
        //         Granted = true
        //     };
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
        var userRoles = await userService.GetUserRoles(request.Token);
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

    public override Task<ValidateTokenResponse> ValidateToken(ValidateTokenRequest request, ServerCallContext context)
    {
        return base.ValidateToken(request, context);
    }

    public override async Task<UserLoginResponse> LoginUser(UserLoginRequest request, ServerCallContext context)
    {
        var mr = await userService.LoginUser(request.Email, request.Password);
        return new UserLoginResponse
        {
            Message = mr.Message,
            Success = mr.IsSuccess,
            Token = mr.Data?.ToString()
        };
    }

    public override async Task<UserRegisterResponse> RegisterUser(UserRegisterRequest request,
        ServerCallContext context)
    {
        var mr = await userService.RegisterUser(new User
        {
            Email = request.Email,
            Password = request.Password,
            Username = request.Nickname,
            Status = Status.Active,
            CreatedDate = DateTime.UtcNow
        });
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