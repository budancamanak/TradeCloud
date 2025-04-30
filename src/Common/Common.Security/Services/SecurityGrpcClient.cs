using Common.Grpc;
using Common.Security.Abstraction;
using Common.Security.Enums;
using Google.Protobuf.Collections;

namespace Common.Security.Services;

public class SecurityGrpcClient(GrpcAuthService.GrpcAuthServiceClient grpcClient) : ISecurityGrpcClient
{
    public async Task<ValidateTokenResponse> ValidateToken(string token)
    {
        var result = await grpcClient.ValidateTokenAsync(new ValidateTokenRequest { Token = token });
        return result;
    }

    public async Task<bool> HasPermissionAsync(string token, params Permissions.Enum[] permissions)
    {
        var rep = new RepeatedField<string>();
        rep.AddRange(permissions.Select(s => s.ToString()));
        var response = await grpcClient.CheckPermissionAsync(new CheckRequest { Token = token, Value = { rep } });
        return response.Granted;
    }

    public async Task<bool> HasRoleAsync(string token, params Roles.Enum[] roles)
    {
        var rep = new RepeatedField<string>();
        rep.AddRange(roles.Select(s => s.ToString()));
        var response = await grpcClient.CheckRoleAsync(new CheckRequest { Token = token, Value = { rep } });
        return response.Granted;
    }

    public async Task<bool> HasScopeAsync(string token, string scope)
    {
        var rep = new RepeatedField<string> { scope };
        var response = await grpcClient.CheckScopeAsync(new CheckRequest() { Token = token, Value = { rep } });
        return response.Granted;
    }

    public async Task<bool> HasPolicyAsync(string token, string policy)
    {
        var rep = new RepeatedField<string> { policy };
        var response = await grpcClient.CheckPolicyAsync(new CheckRequest() { Token = token, Value = { rep } });
        return response.Granted;
    }
}