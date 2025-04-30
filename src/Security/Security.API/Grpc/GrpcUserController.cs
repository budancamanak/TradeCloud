using AutoMapper;
using Common.Grpc;
using Grpc.Core;
using MediatR;
using Security.Application.Features.Checks;
using Security.Application.Features.Checks.PermissionCheck;
using Security.Application.Features.Checks.RoleCheck;
using Security.Application.Features.User.LoginUser;
using Security.Application.Features.User.RegisterUser;

namespace Security.API.Grpc;

public class GrpcUserController(IMapper mapper, IMediator mediator) : GrpcUserService.GrpcUserServiceBase
{
    public override async Task<UserLoginResponse> LoginUser(UserLoginRequest grpcRequest, ServerCallContext context)
    {
        var request = mapper.Map<LoginUserRequest>(grpcRequest,
            opts => { opts.Items["ClientIP"] = (context.RequestHeaders.GetValue("ClientIP") ?? ""); });
        var mr = await mediator.Send(request);
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