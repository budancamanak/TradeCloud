using Common.Grpc;
using Grpc.Core;

namespace Security.API.Grpc;

public class SecurityGrpcController : GrpcAuthController.GrpcAuthControllerBase
{
    public override Task<CheckResponse> CheckPermission(CheckRequest request, ServerCallContext context)
    {
        return base.CheckPermission(request, context);
    }

    public override Task<CheckResponse> CheckPolicy(CheckRequest request, ServerCallContext context)
    {
        return base.CheckPolicy(request, context);
    }

    public override Task<CheckResponse> CheckRole(CheckRequest request, ServerCallContext context)
    {
        return base.CheckRole(request, context);
    }

    public override Task<CheckResponse> CheckScope(CheckRequest request, ServerCallContext context)
    {
        return base.CheckScope(request, context);
    }

    public override Task<ValidateTokenResponse> ValidateToken(ValidateTokenRequest request, ServerCallContext context)
    {
        return base.ValidateToken(request, context);
    }

    public override Task<UserLoginResponse> LoginUser(UserLoginRequest request, ServerCallContext context)
    {
        return base.LoginUser(request, context);
    }

    public override Task<UserRegisterResponse> RegisterUser(UserRegisterRequest request, ServerCallContext context)
    {
        return base.RegisterUser(request, context);
    }

    public override Task<UserInfoResponse> UserInfo(UserInfoRequest request, ServerCallContext context)
    {
        return base.UserInfo(request, context);
    }
}