using Ardalis.GuardClauses;
using Common.Core.Extensions;
using Common.Core.Models;
using Common.Security.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Security.Application.Abstraction.Repositories;
using Security.Domain.Entities;
using Security.Infrastructure.Data;

namespace Security.Infrastructure.Repositories;

public class RoleRepository(SecurityDbContext dbContext) : IRoleRepository
{
    public async Task<Role> GetByIdAsync(int id)
    {
        Guard.Against.NegativeOrZero(id);
        var role = await dbContext.Roles.FirstOrDefaultAsync(f => f.Id == id);
        Guard.Against.Null(role);
        return role;
    }

    public async Task<List<Role>> GetAllAsync()
    {
        return await dbContext.Roles.ToListAsync();
    }

    public Task<MethodResponse> AddAsync(Role item)
    {
        throw new NotImplementedException();
    }

    public Task<MethodResponse> UpdateAsync(Role item)
    {
        throw new NotImplementedException();
    }

    public Task<MethodResponse> DeleteAsync(Role item)
    {
        throw new NotImplementedException();
    }

    public Task<MethodResponse> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<MethodResponse> AddPermissionToRole(Roles role, Permissions permission)
    {
        Guard.Against.Null(role);
        Guard.Against.Null(permission);
        var existing = await dbContext.Roles.Include(f => f.Permissions).FirstOrDefaultAsync(f => f.Id == role.Value);
        Guard.Against.Null(existing);
        var existingPermission = existing.Permissions.FirstOrDefault(f => f.Id == permission.Value);
        Guard.Against.NonNull(existingPermission);
        existing.Permissions.Add(new Permission
        {
            Id = permission.Value,
            Name = permission.Name
        });
        var result = await dbContext.SaveChangesAsync();
        if (result == 0) return MethodResponse.Error("Failed to add permission to role");
        return MethodResponse.Success(result, "Permission added to role");
    }

    public async Task<MethodResponse> RemovePermissionFromRole(Roles role, Permissions permission)
    {
        Guard.Against.Null(role);
        Guard.Against.Null(permission);
        var existing = await dbContext.Roles.Include(f => f.Permissions).FirstOrDefaultAsync(f => f.Id == role.Value);
        Guard.Against.Null(existing);
        var existingPermission = existing.Permissions.FirstOrDefault(f => f.Id == permission.Value);
        Guard.Against.Null(existingPermission);
        existing.Permissions.Remove(existingPermission);
        var result = await dbContext.SaveChangesAsync();
        if (result == 0) return MethodResponse.Error("Failed to remove permission from role");
        return MethodResponse.Success(result, "Permission removed from role");
    }
}