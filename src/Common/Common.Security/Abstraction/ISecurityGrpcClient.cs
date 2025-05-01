using Common.Grpc;
using Common.Security.Enums;

namespace Common.Security.Abstraction;

public interface ISecurityGrpcClient
{
    Task<GrpcValidateTokenResponse> ValidateToken(string token);
    Task<bool> HasPermissionAsync(string token, params Permissions.Enum[] permissions);
    Task<bool> HasRoleAsync(string token, params Roles.Enum[] roles);
    Task<bool> HasScopeAsync(string token, string scope);
    Task<bool> HasPolicyAsync(string token, string policy);
}