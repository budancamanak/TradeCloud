using Common.Application.Repositories;
using Common.Core.Models;
using Security.Domain.Entities;

namespace Security.Application.Abstraction.Repositories;

public interface IUserRoleRepository : IAsyncRepository<UserRole>
{
    Task<List<UserRole>> GetUserRoles(string token);
    Task<List<UserRole>> GetUserRoles(int userId);
    Task<MethodResponse> RemoveRoleFromUser(int userId, int roleId);
}