namespace Common.Security.Interfaces;

public interface ISecurityGrpcClient
{
    Task<bool> HasPermissionAsync(string token, string permission);
    Task<bool> HasRoleAsync(string token, string role);
    Task<bool> HasScopeAsync(string token, string scope);
    Task<bool> HasPolicyAsync(string token, string policy);
}