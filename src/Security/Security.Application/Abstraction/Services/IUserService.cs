using Common.Core.Models;
using Security.Domain.Entities;

namespace Security.Application.Abstraction.Services;

public interface IUserService
{
    Task<MethodResponse> RegisterUser(User user);
    Task<MethodResponse> LoginUser(string username, string password,string clientIp);
    Task<MethodResponse> LogoutUser(string token);
    Task<List<Permission>> GetUserPermissions(string token);
    Task<List<Role>> GetUserRoles(string token);
}