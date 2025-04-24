using Common.Application.Repositories;
using Common.Core.Models;
using Security.Domain.Entities;

namespace Security.Application.Abstraction.Repositories;

public interface IUserRepository : IAsyncRepository<User>
{
    Task<MethodResponse> RegisterUser(User user);
    Task<MethodResponse> LoginUser(string username, string password);
    Task<MethodResponse> LogoutUser(string token);
    Task<MethodResponse> CheckUsernameAvailability(string username);
    Task<MethodResponse> UpdateUserStatus(string username);
    Task<List<Permission>> GetUserPermissions(string token);
}