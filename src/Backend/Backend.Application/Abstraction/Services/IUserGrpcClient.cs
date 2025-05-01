using Common.Core.Models;

namespace Backend.Application.Abstraction.Services;

public interface IUserGrpcClient
{
    Task<MethodResponse> LoginUserAsync(string email, string password);
    Task<MethodResponse> RegisterUserAsync(string username, string email, string password, string passwordConfirm);
    Task<MethodResponse> UserInfoAsync(string token);
    Task<MethodResponse> AddRoleToUserAsync(string token, int roleId);
    Task<MethodResponse> RemoveRoleFromUserAsync(string token, int roleId);
    Task<MethodResponse> AddPermissionToRoleAsync(string token, int roleId, int permissionId);
    Task<MethodResponse> RemovePermissionFromRoleAsync(string token, int roleId, int permissionId);
}