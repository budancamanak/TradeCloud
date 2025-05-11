using AutoMapper;
using Common.Grpc;
using Grpc.Core;
using MediatR;
using Security.Application.Features.User.AddPermissionToRole;
using Security.Application.Features.User.AddRoleToUser;
using Security.Application.Features.User.LoginUser;
using Security.Application.Features.User.RegisterUser;
using Security.Application.Features.User.RemovePermissionFromRole;
using Security.Application.Features.User.RemoveRoleFromUser;

namespace Security.API.Grpc;

public class GrpcUserController(IMapper mapper, IMediator mediator) : GrpcUserService.GrpcUserServiceBase
{
    public override async Task<GrpcUserLoginResponse> LoginUser(GrpcUserLoginRequest grpcRequest,
        ServerCallContext context)
    {
        var request = mapper.Map<LoginUserRequest>(grpcRequest,
            opts => { opts.Items["ClientIP"] = (context.RequestHeaders.GetValue("ClientIP") ?? ""); });
        var mr = await mediator.Send(request);
        return new GrpcUserLoginResponse
        {
            Message = mr.Message,
            Success = mr.IsSuccess,
            Token = mr.Data?.ToString() ?? "Empty"
        };
    }

    public override async Task<GrpcUserRegisterResponse> RegisterUser(GrpcUserRegisterRequest grpcRequest,
        ServerCallContext context)
    {
        var request = mapper.Map<RegisterUserRequest>(grpcRequest);
        var mr = await mediator.Send(request, context.CancellationToken);
        return new GrpcUserRegisterResponse
        {
            Message = mr.Message,
            Success = mr.IsSuccess
        };
    }

    public override Task<GrpcUserInfoResponse> UserInfo(GrpcUserInfoRequest request, ServerCallContext context)
    {
        return base.UserInfo(request, context);
    }

    public override async Task<GrpcMethodResponse> AddRoleToUser(GrpcAddRoleToUserRequest grpcRequest,
        ServerCallContext context)
    {
        var request = new AddRoleToUserRequest
        {
            RoleId = grpcRequest.RoleId,
            UserId = grpcRequest.UserId
        };
        var mr = await mediator.Send(request, context.CancellationToken);
        return new GrpcMethodResponse
        {
            Message = mr.Message,
            Success = mr.IsSuccess
        };
    }

    public override async Task<GrpcMethodResponse> RemoveRoleFromUser(GrpcRemoveRoleFromUserRequest grpcRequest,
        ServerCallContext context)
    {
        var request = new RemoveRoleFromUserRequest
        {
            RoleId = grpcRequest.RoleId,
            UserId = grpcRequest.UserId
        };
        var mr = await mediator.Send(request, context.CancellationToken);
        return new GrpcMethodResponse
        {
            Message = mr.Message,
            Success = mr.IsSuccess
        };
    }

    public override async Task<GrpcMethodResponse> AddPermissionToRole(GrpcAddPermissionToRoleRequest grpcRequest,
        ServerCallContext context)
    {
        var request = new AddPermissionToRoleRequest
        {
            RoleId = grpcRequest.RoleId,
            PermissionId = grpcRequest.PermissionId
        };
        var mr = await mediator.Send(request, context.CancellationToken);
        return new GrpcMethodResponse
        {
            Message = mr.Message,
            Success = mr.IsSuccess
        };
    }

    public override async Task<GrpcMethodResponse> RemovePermissionFromRole(
        GrpcRemovePermissionFromRoleRequest grpcRequest,
        ServerCallContext context)
    {
        var request = new RemovePermissionFromRoleRequest
        {
            RoleId = grpcRequest.RoleId,
            PermissionId = grpcRequest.PermissionId
        };
        var mr = await mediator.Send(request, context.CancellationToken);
        return new GrpcMethodResponse
        {
            Message = mr.Message,
            Success = mr.IsSuccess
        };
    }
}