using Common.Grpc;
using Common.Security.Abstraction;

namespace Common.Security.Services;

public class SecurityGrpcClient(GrpcAuthService.GrpcAuthServiceClient grpcClient) : ISecurityGrpcClient
{
    public async Task<ValidateTokenResponse> ValidateToken(string token)
    {
        var result = await grpcClient.ValidateTokenAsync(new ValidateTokenRequest { Token = token });
        return result;
    }

    public async Task<bool> HasPermissionAsync(string token, string permission)
    {
        var response = await grpcClient.CheckPermissionAsync(new CheckRequest() { Token = token, Value = permission });
        return response.Granted;
    }

    public async Task<bool> HasRoleAsync(string token, string role)
    {
        var response = await grpcClient.CheckRoleAsync(new CheckRequest() { Token = token, Value = role });
        return response.Granted;
    }

    public async Task<bool> HasScopeAsync(string token, string scope)
    {
        var response = await grpcClient.CheckScopeAsync(new CheckRequest() { Token = token, Value = scope });
        return response.Granted;
    }

    public async Task<bool> HasPolicyAsync(string token, string policy)
    {
        var response = await grpcClient.CheckPolicyAsync(new CheckRequest() { Token = token, Value = policy });
        return response.Granted;
    }
}