using Common.Application.Repositories;
using Common.Core.Models;
using Common.Security.Enums;
using Security.Domain.Entities;

namespace Security.Application.Abstraction.Repositories;

public interface IRoleRepository : IAsyncRepository<Role>
{
    Task<MethodResponse> AddPermissionToRole(Roles role, Permissions permission);
    Task<MethodResponse> RemovePermissionFromRole(Roles role, Permissions permission);
}