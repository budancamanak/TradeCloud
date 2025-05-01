using Ardalis.GuardClauses;
using Common.Application.Repositories;
using Common.Application.Services;
using Common.Core.Models;
using Common.Core.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
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
        try
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
        catch (Exception e)
        {
            return MethodResponse.Error(e.Message);
        }
    }

    public async Task<MethodResponse> LoginUser(string username, string password, string clientIp)
    {
        try
        {
            var user = await repository.FindUserByUsername(username);
            Guard.Against.Null(user);
            var passMatch = BCrypt.Net.BCrypt.Verify(password, user.Password);
            if (!passMatch) return MethodResponse.Error("Password mismatch");
            // todo generate jwt token
            var token = tokenService.GenerateToken(user, clientIp);
            Guard.Against.NullOrEmpty(token,
                exceptionCreator: () => new SecurityTokenEncryptionFailedException("Failed to create token"));
            var expDate = DateTime.UtcNow.AddMinutes(configuration.GetValue<int>("Jwt:Expiration"));
            var loginInfo = new UserLogin
            {
                Token = token,
                ExpirationDate = expDate,
                LoginDate = DateTime.UtcNow,
                UserAgent = "UserAgent",
                UserId = user.Id,
                ClientIP = clientIp
            };
            var mr = await repository.AddUserLogin(user, loginInfo);
            await cache.SetAsync(CacheKeyGenerator.UserRoleInfoKey(user.Id.ToString()),
                JsonConvert.SerializeObject(user.UserRoles),
                TimeSpan.FromMinutes(15));
            await cache.SetAsync(CacheKeyGenerator.UserTokenInfoKey(token), loginInfo, TimeSpan.FromMinutes(15));
            return mr.WithData(token);
        }
        catch (Exception e)
        {
            return MethodResponse.Error(e.Message);
        }
    }

    public async Task<MethodResponse> LogoutUser(string token)
    {
        // todo set UserLogin as expired as well.
        var loginInfo = await cache.GetAsync<UserLogin>(CacheKeyGenerator.UserTokenInfoKey(token));
        if (loginInfo != null)
        {
            loginInfo.IsLoggedOut = true;
        }

        await cache.SetAsync(CacheKeyGenerator.UserTokenInfoKey(token), loginInfo, TimeSpan.FromSeconds(1));

        loginInfo = await repository.GetUserLoginInfo(token);
        if (loginInfo != null)
        {
            await repository.SetUserLoginInfoLoggedOut(token, true);
        }

        return MethodResponse.Success("User logged out");
    }

    public async Task<List<Permission>> GetUserPermissions(string userId)
    {
        try
        {
            var cached = await cache.GetAsync<List<Permission>>(CacheKeyGenerator.UserPermissionsKey(userId));
            if (cached is { Count: > 0 }) return cached;
            cached = await repository.GetUserPermissions(int.Parse(userId));
            await cache.SetAsync(CacheKeyGenerator.UserPermissionsKey(userId), cached,
                TimeSpan.FromMinutes(15));
            return cached;
        }
        catch (Exception e)
        {
            return [];
        }
    }

    public async Task<List<Role>> GetUserRoles(string userId)
    {
        try
        {
            var cached = await cache.GetAsync<List<Role>>(CacheKeyGenerator.UserRoleInfoKey(userId));
            if (cached is { Count: > 0 }) return cached;
            cached = await repository.GetUserRoles(int.Parse(userId));
            await cache.SetAsync(CacheKeyGenerator.UserRoleInfoKey(userId), cached,
                TimeSpan.FromMinutes(15));
            return cached;
        }
        catch (Exception e)
        {
            return [];
        }
    }
}