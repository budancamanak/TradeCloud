using Ardalis.GuardClauses;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Models;
using Common.Security.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Security.Application.Abstraction.Repositories;
using Security.Domain.Entities;
using Security.Infrastructure.Data;

namespace Security.Infrastructure.Repositories;

public class UserRepository(SecurityDbContext dbContext, IValidator<User> validator) : IUserRepository
{
    public async Task<User> GetByIdAsync(int id)
    {
        Guard.Against.NegativeOrZero(id);
        var user = await dbContext.Users.FirstOrDefaultAsync(f => f.Id == id);
        Guard.Against.Null(user);
        return user;
    }

    public async Task<List<User>> GetUsersWithStatus(Status status)
    {
        // Guard.Against.EnumOutOfRange<Status>(status);
        Guard.Against.Null(status);
        return await dbContext.Users.Where(f => f.Status == status).ToListAsync();
    }

    public async Task<User> FindUserByUsername(string username)
    {
        Guard.Against.NullOrWhiteSpace(username);
        var item = await dbContext.Users.FirstOrDefaultAsync(f => f.Username == username);
        Guard.Against.Null(item);
        return item;
    }

    public async Task<User> FindUserByEmail(string email)
    {
        Guard.Against.NullOrWhiteSpace(email);
        var item = await dbContext.Users.FirstOrDefaultAsync(f => f.Email == email);
        Guard.Against.Null(item);
        return item;
    }

    public async Task<MethodResponse> AddUserLogin(User user, UserLogin login)
    {
        Guard.Against.Null(user);
        Guard.Against.Null(login);
        user.UserLogins.Add(login);
        var result = await dbContext.SaveChangesAsync();
        if (result == 0) return MethodResponse.Error("Failed to save user login");
        return MethodResponse.Success(user.Id, "User login saved");
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await dbContext.Users.ToListAsync();
    }

    public async Task<MethodResponse> AddAsync(User item)
    {
        Guard.Against.Null(item);
        await validator.ValidateAndThrowAsync(item);
        var existing =
            await dbContext.Users.FirstOrDefaultAsync(f => f.Email == item.Email || f.Username == item.Username);
        Guard.Against.NonNull(existing, "User already registered");
        item.Password = BCrypt.Net.BCrypt.HashPassword(item.Password, 12);
        dbContext.Users.Add(item);
        var result = await dbContext.SaveChangesAsync();
        if (result == 0) return MethodResponse.Error("Failed to save user");
        return MethodResponse.Success(item.Id, "User saved");
    }

    public async Task<MethodResponse> UpdateAsync(User item)
    {
        Guard.Against.Null(item);
        Guard.Against.NegativeOrZero(item.Id);
        // todo implement User validators
        await validator.ValidateAndThrowAsync(item);
        var existing = await dbContext.Users.FirstOrDefaultAsync(f => f.Id == item.Id);
        Guard.Against.NotFound(item.Id, existing);

        existing.Username = item.Username;
        existing.Status = item.Status;
        var result = await dbContext.SaveChangesAsync();
        if (result == 0) return MethodResponse.Error("Failed to update User");
        return MethodResponse.Success(item.Id, "User updated");
    }

    public async Task<MethodResponse> DeleteAsync(User item)
    {
        Guard.Against.Null(item);
        Guard.Against.NegativeOrZero(item.Id);
        var existing = await dbContext.Users.FirstOrDefaultAsync(f => f.Id == item.Id);
        Guard.Against.NotFound(item.Id, existing);
        if (item.Username != existing.Username || item.Email != existing.Email)
            throw new ArgumentException("User info mismatch");
        return await _DeleteUser(existing);
    }

    public async Task<MethodResponse> DeleteAsync(int id)
    {
        Guard.Against.NegativeOrZero(id);
        var existing = await dbContext.Users.FirstOrDefaultAsync(f => f.Id == id);
        Guard.Against.NotFound(id, existing);
        return await _DeleteUser(existing);
    }

    private async Task<MethodResponse> _DeleteUser(User existing)
    {
        dbContext.Users.Remove(existing);
        var result = await dbContext.SaveChangesAsync();
        if (result == 0) return MethodResponse.Error("Failed to delete User");
        return MethodResponse.Success(existing.Id, "User deleted");
    }

    public async Task<MethodResponse> CheckUsernameAvailability(string username)
    {
        Guard.Against.NullOrWhiteSpace(username);
        var count = await dbContext.Users.CountAsync(f => f.Username == username);
        return count == 0
            ? MethodResponse.Error("Username is not being used")
            : MethodResponse.Success("Username is already taken");
    }

    public async Task<MethodResponse> CheckEmailAvailability(string email)
    {
        Guard.Against.NullOrWhiteSpace(email);
        var count = await dbContext.Users.CountAsync(f => f.Email == email);
        return count == 0
            ? MethodResponse.Error("Email is not being used")
            : MethodResponse.Success("Email is already taken");
    }

    public async Task<MethodResponse> UpdateUserPassword(int id, string password)
    {
        Guard.Against.NegativeOrZero(id);
        Guard.Against.NullOrWhiteSpace(password);
        var existing =
            await dbContext.Users.FirstOrDefaultAsync(f => f.Id == id);
        Guard.Against.Null(existing, "User not found");
        existing.Password = BCrypt.Net.BCrypt.HashPassword(password, 12);
        var result = await dbContext.SaveChangesAsync();
        if (result == 0) return MethodResponse.Error("Failed to update password");
        return MethodResponse.Success(id, "Password updated");
    }

    public async Task<MethodResponse> UpdateUserStatus(int id, Status status)
    {
        Guard.Against.NegativeOrZero(id);
        Guard.Against.Null(status);
        var existing = await dbContext.Users.FirstOrDefaultAsync(f => f.Id == id);
        Guard.Against.Null(existing);
        existing.Status = status;
        var result = await dbContext.SaveChangesAsync();
        if (result == 0) return MethodResponse.Error("Failed to update user status");
        return MethodResponse.Success(existing.Id, "User status updated");
    }

    public async Task<MethodResponse> AddRoleToUser(int userId, Roles eRole)
    {
        Guard.Against.NegativeOrZero(userId);
        var item = await dbContext.Users.Include(user => user.UserRoles).FirstOrDefaultAsync(f => f.Id == userId);
        Guard.Against.Null(item);
        var role = await dbContext.Roles.FirstOrDefaultAsync(f => f.Id == eRole.Value);
        Guard.Against.Null(role);
        item.UserRoles.Add(role);
        var result = await dbContext.SaveChangesAsync();
        if (result == 0) return MethodResponse.Error("Failed to assign role to user");
        return MethodResponse.Success(userId, "Role assigned to user");
    }

    public async Task<List<Role>> GetUserRoles(int userId)
    {
        Guard.Against.NegativeOrZero(userId);
        var item = await dbContext.Users.Include(user => user.UserRoles).FirstOrDefaultAsync(f => f.Id == userId);
        Guard.Against.Null(item);
        return item.UserRoles.ToList();
    }

    public async Task<List<Permission>> GetUserPermissions(int userId)
    {
        Guard.Against.NegativeOrZero(userId);
        var user = await dbContext.Users.Include(user => user.UserRoles).ThenInclude(f => f.Permissions)
            .FirstOrDefaultAsync(f => f.Id == userId);
        Guard.Against.Null(user);
        var permissions = new List<Permission>();
        foreach (var userRole in user.UserRoles)
        {
            permissions.AddRange(userRole.Permissions);
        }

        return permissions.Distinct().ToList();
    }

    public async Task<MethodResponse> RemoveRoleFromUser(int userId, int roleId)
    {
        Guard.Against.NegativeOrZero(userId);
        var user = await dbContext.Users.Include(user => user.UserRoles).FirstOrDefaultAsync(f => f.Id == userId);
        Guard.Against.Null(user);
        var item = user.UserRoles.FirstOrDefault(f => f.Id == roleId);
        Guard.Against.Null(item);
        user.UserRoles.Remove(item);
        var result = await dbContext.SaveChangesAsync();
        if (result == 0) return MethodResponse.Error("Failed to remove role from user");
        return MethodResponse.Success(user.Id, "Role removed from user");
    }

    public async Task<UserLogin?> GetUserLoginInfo(string token)
    {
        Guard.Against.NullOrWhiteSpace(token);
        var info = await dbContext.UserLogins.FirstOrDefaultAsync(f => f.Token == token);
        Guard.Against.Null(info);
        return info;
    }

    public async Task<MethodResponse> SetUserLoginInfoLoggedOut(string token, bool loggedOut)
    {
        Guard.Against.NullOrEmpty(token);
        var existing = await dbContext.UserLogins.FirstOrDefaultAsync(f => f.Token == token);
        Guard.Against.Null(existing);
        existing.IsLoggedOut = loggedOut;
        var result = await dbContext.SaveChangesAsync();
        if (result == 0) return MethodResponse.Error("Failed to update User Login");
        return MethodResponse.Success(result, "User Login updated");
    }
}