using Common.Application.Repositories;
using Common.Core.Enums;
using Common.Core.Models;
using Security.Domain.Entities;

namespace Security.Application.Abstraction.Repositories;

public interface IUserRepository : IAsyncRepository<User>
{
    Task<List<User>> GetUsersWithStatus(Status status);
    Task<User> FindUserByUsername(string username);
    Task<MethodResponse> AddUserLogin(User user, UserLogin login);
    Task<MethodResponse> CheckUsernameAvailability(string username);
    Task<MethodResponse> CheckEmailAvailability(string email);
    Task<MethodResponse> UpdateUserPassword(int id, string password);
    Task<MethodResponse> UpdateUserStatus(int id, Status status);
    Task<List<Role>> GetUserRoles(string token);
    Task<List<Role>> GetUserRoles(int userId);
    Task<MethodResponse> RemoveRoleFromUser(int userId, int roleId);
}