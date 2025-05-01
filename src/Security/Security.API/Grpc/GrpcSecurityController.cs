using AutoMapper;
using Common.Grpc;
using Grpc.Core;
using MediatR;
using Security.Application.Features.Checks;
using Security.Application.Features.Checks.PermissionCheck;
using Security.Application.Features.Checks.RoleCheck;
using Security.Application.Features.Checks.TokenCheck;
using Security.Application.Features.User.LoginUser;
using Security.Application.Features.User.RegisterUser;

namespace Security.API.Grpc;

public class GrpcSecurityController(IMapper mapper, IMediator mediator)
    : GrpcAuthService.GrpcAuthServiceBase
{
    public override async Task<CheckResponse> CheckPermission(CheckRequest grpcRequest, ServerCallContext context)
    {
        // todo use automapper here
        var request = new PermissionCheckRequest
        {
            Permissions = grpcRequest.Value.ToList(),
            Token = grpcRequest.Token,
            ClientIp = context.RequestHeaders.GetValue("ClientIP") ?? ""
        };
        var mr = await mediator.Send(request);
        // todo return message here as well to inform back?
        return new CheckResponse
        {
            Granted = mr.IsSuccess
        };
    }

    public override Task<CheckResponse> CheckPolicy(CheckRequest request, ServerCallContext context)
    {
        return base.CheckPolicy(request, context);
    }

    public override async Task<CheckResponse> CheckRole(CheckRequest grpcRequest, ServerCallContext context)
    {
        var request = new RoleCheckRequest
        {
            Roles = grpcRequest.Value.ToList(),
            Token = grpcRequest.Token,
            ClientIp = context.RequestHeaders.GetValue("ClientIP") ?? ""
        };
        var mr = await mediator.Send(request);
        // todo return message here as well to inform back?
        return new CheckResponse
        {
            Granted = mr.IsSuccess
        };
    }

    public override Task<CheckResponse> CheckScope(CheckRequest request, ServerCallContext context)
    {
        return base.CheckScope(request, context);
    }

    public override async Task<ValidateTokenResponse> ValidateToken(ValidateTokenRequest grpcRequest,
        ServerCallContext context)
    {
        var request = new TokenCheckRequest
        {
            Token = grpcRequest.Token,
            ClientIp = context.RequestHeaders.GetValue("ClientIP") ?? ""
        };
        return await mediator.Send(request);
    }
}