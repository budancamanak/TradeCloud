using AutoMapper;
using Common.Grpc;
using Grpc.Core;
using MediatR;
using Security.Application.Features.Checks.PermissionCheck;
using Security.Application.Features.Checks.RoleCheck;
using Security.Application.Features.Checks.TokenCheck;

namespace Security.API.Grpc;

public class GrpcSecurityController(IMapper mapper, IMediator mediator)
    : GrpcAuthService.GrpcAuthServiceBase
{
    public override async Task<GrpcCheckResponse> CheckPermission(GrpcCheckRequest grpcRequest, ServerCallContext context)
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
        return new GrpcCheckResponse
        {
            Granted = mr.IsSuccess
        };
    }

    public override Task<GrpcCheckResponse> CheckPolicy(GrpcCheckRequest request, ServerCallContext context)
    {
        return base.CheckPolicy(request, context);
    }

    public override async Task<GrpcCheckResponse> CheckRole(GrpcCheckRequest grpcRequest, ServerCallContext context)
    {
        var request = new RoleCheckRequest
        {
            Roles = grpcRequest.Value.ToList(),
            Token = grpcRequest.Token,
            ClientIp = context.RequestHeaders.GetValue("ClientIP") ?? ""
        };
        var mr = await mediator.Send(request);
        // todo return message here as well to inform back?
        return new GrpcCheckResponse
        {
            Granted = mr.IsSuccess
        };
    }

    public override Task<GrpcCheckResponse> CheckScope(GrpcCheckRequest request, ServerCallContext context)
    {
        return base.CheckScope(request, context);
    }

    public override async Task<GrpcValidateTokenResponse> ValidateToken(GrpcValidateTokenRequest grpcRequest,
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