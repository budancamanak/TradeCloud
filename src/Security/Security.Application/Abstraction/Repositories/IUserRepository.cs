using Common.Application.Repositories;
using Common.Core.Enums;
using Common.Core.Models;
using Common.Security.Enums;
using Security.Domain.Entities;

namespace Security.Application.Abstraction.Repositories;

public interface IUserRepository : IAsyncRepository<User>
{
    Task<List<User>> GetUsersWithStatus(Status status);
    Task<User> FindUserByUsername(string username);
    Task<User> FindUserByEmail(string email);
    Task<MethodResponse> AddUserLogin(User user, UserLogin login);
    Task<MethodResponse> CheckUsernameAvailability(string username);
    Task<MethodResponse> CheckEmailAvailability(string email);
    Task<MethodResponse> UpdateUserPassword(int id, string password);
    Task<MethodResponse> UpdateUserStatus(int id, Status status);
    Task<List<Role>> GetUserRoles(int userId);
    Task<List<Permission>> GetUserPermissions(int userId);
    Task<MethodResponse> AddRoleToUser(int userId, Roles eRole);
    Task<MethodResponse> RemoveRoleFromUser(int userId, int roleId);
    Task<UserLogin?> GetUserLoginInfo(string token);
    Task<MethodResponse> SetUserLoginInfoLoggedOut(string token, bool loggedOut);
}