using Ardalis.GuardClauses;
using Common.Application.Repositories;
using Common.Application.Services;
using Common.Core.Models;
using Common.Core.Serialization;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Security.Application.Abstraction.Repositories;
using Security.Application.Abstraction.Services;
using Security.Domain.Entities;

namespace Security.Infrastructure.Services;

public class UserService(
    IUserRepository repository,
    ITokenService tokenService,
    ICacheService cache,
    IConfiguration configuration) : IUserService
{
    public async Task<MethodResponse> RegisterUser(User user)
    {
        var mr = await repository.CheckUsernameAvailability(user.Username);
        if (mr.IsSuccess) return MethodResponse.Error(mr.Message);
        mr = await repository.CheckEmailAvailability(user.Email);
        if (mr.IsSuccess) return MethodResponse.Error(mr.Message);
        // todo check password if its strong
        // todo send registration email.
        mr = await repository.AddAsync(user);
        return mr;
    }

    public async Task<MethodResponse> LoginUser(string username, string password, string clientIp)
    {
        var user = await repository.FindUserByUsername(username);
        Guard.Against.Null(user);
        var passMatch = BCrypt.Net.BCrypt.Verify(password, user.Password);
        if (!passMatch) return MethodResponse.Error("Password mismatch");
        // todo generate jwt token
        var token = tokenService.GenerateToken(user, clientIp);
        var expDate = DateTime.UtcNow.AddMinutes(configuration.GetValue<int>("Jwt:Expiration"));
        var mr = await repository.AddUserLogin(user, new UserLogin
        {
            Token = token,
            ExpirationDate = expDate,
            LoginDate = DateTime.UtcNow,
            UserAgent = "UserAgent",
            UserId = user.Id,
            ClientIP = clientIp
        });
        await cache.SetAsync(CacheKeyGenerator.UserRoleInfoKey(user.Id.ToString()),
            JsonConvert.SerializeObject(user.UserRoles),
            TimeSpan.FromMinutes(15));
        return mr.WithData(token);
    }

    public Task<MethodResponse> LogoutUser(string token)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Permission>> GetUserPermissions(string userId)
    {
        // var cached = await cache.GetAsync<List<Permission>>(CacheKeyGenerator.UserPermissionsKey(userId));
        // if (cached is { Count: > 0 }) return cached;
        var cached = await repository.GetUserPermissions(int.Parse(userId));
        // await cache.SetAsync(CacheKeyGenerator.UserPermissionsKey(userId), JsonSerialization.ToJson(cached),
        //     TimeSpan.FromMinutes(15));
        return cached;
    }

    public async Task<List<Role>> GetUserRoles(string userId)
    {
        // var cached = await cache.GetAsync<List<Role>>(CacheKeyGenerator.UserRoleInfoKey(userId));
        // if (cached is { Count: > 0 }) return cached;
        var cached = await repository.GetUserRoles(int.Parse(userId));
        // await cache.SetAsync(CacheKeyGenerator.UserRoleInfoKey(userId), JsonSerialization.ToJson(cached),
        //     TimeSpan.FromMinutes(15));
        return cached;
    }
}