using Common.Application.Repositories;
using Security.Domain.Entities;

namespace Security.Application.Abstraction.Repositories;

public interface IUserRoleRepository : IAsyncRepository<UserRole>
{
    Task<List<UserRole>> GetUserRolesByToken(string token);
    Task<List<UserRole>> GetUserRoles(string identity);
    Task<List<UserRole>> GetUserRoles(int userId);
}