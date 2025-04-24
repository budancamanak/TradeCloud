using Ardalis.GuardClauses;
using Common.Core.Models;
using Security.Application.Abstraction.Repositories;
using Security.Application.Abstraction.Services;
using Security.Domain.Entities;

namespace Security.Infrastructure.Services;

public class UserService(IUserRepository repository, ITokenService tokenService) : IUserService
{
    public async Task<MethodResponse> RegisterUser(User user)
    {
        var mr = await repository.CheckUsernameAvailability(user.Username);
        if (mr.IsSuccess) return mr;
        // todo check email availability
        // todo check password if its strong
        // todo send registration email.
        mr = await repository.AddAsync(user);
        return mr;
    }

    public async Task<MethodResponse> LoginUser(string username, string password)
    {
        var user = await repository.FindUserByUsername(username);
        Guard.Against.Null(user);
        var passMatch = BCrypt.Net.BCrypt.Verify(password, user.Password);
        if (!passMatch) return MethodResponse.Error("Password mismatch");
        // todo generate jwt token
        var token = tokenService.GenerateToken();
        var mr = await repository.AddUserLogin(user, new UserLogin
        {
            Token = token,
            ExpirationDate = DateTime.UtcNow.AddDays(7),
            LoginDate = DateTime.UtcNow,
            UserAgent = "UserAgent",
            UserId = user.Id,
            ClientIP = "localhost"
        });
        return mr.WithData(token);
    }

    public Task<MethodResponse> LogoutUser(string token)
    {
        throw new NotImplementedException();
    }

    public Task<List<Permission>> GetUserPermissions(string token)
    {
        throw new NotImplementedException();
    }
}