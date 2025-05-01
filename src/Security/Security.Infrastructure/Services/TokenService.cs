using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Common.Application.Repositories;
using Common.Application.Services;
using Common.Grpc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Security.Application.Abstraction.Repositories;
using Security.Application.Abstraction.Services;
using Security.Domain.Entities;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;


namespace Security.Infrastructure.Services;

public sealed class TokenService(IConfiguration configuration, ICacheService cache, IUserRepository repository)
    : ITokenService
{
    public string GenerateToken(User user, string clientIp)
    {
        try
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Nickname, user.Username),
                    new Claim(JwtRegisteredClaimNames.Address, clientIp),
                    new Claim("AppAuthor", "budancamanak")
                ]),
                Expires = DateTime.UtcNow.AddMinutes(configuration.GetValue<int>("Jwt:Expiration")),
                SigningCredentials = credentials,
                Issuer = configuration["Jwt:Issuer"],
                Audience = configuration["Jwt:Audience"]
            };
            var handler = new JsonWebTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);
            return token;
        }
        catch (Exception e)
        {
            return string.Empty;
        }
    }

    public async Task<GrpcValidateTokenResponse> ValidateToken(string token, string clientIp)
    {
        try
        {
// todo send below to DI
            var handler = new JwtSecurityTokenHandler();
            var pars = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!)),
                ClockSkew = TimeSpan.Zero
            };
            var tokenValidationResult = await handler.ValidateTokenAsync(token, pars);
            if (!tokenValidationResult.IsValid || tokenValidationResult.Exception != null)
            {
                return new GrpcValidateTokenResponse
                {
                    IsValid = false,
                    UserId = tokenValidationResult.Exception.Message
                };
            }

            if (!tokenValidationResult.ClaimsIdentity.IsAuthenticated)
            {
                return new GrpcValidateTokenResponse
                {
                    IsValid = false,
                    UserId = "Token is valid but not authenticated"
                };
            }

            var user = tokenValidationResult.ClaimsIdentity.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrWhiteSpace(user))
            {
                return new GrpcValidateTokenResponse
                {
                    IsValid = false,
                    UserId = "Token is valid but not authenticated"
                };
            }

            var issuedIp = tokenValidationResult.ClaimsIdentity.FindFirst(JwtRegisteredClaimNames.Address)?.Value;
            // todo enabled below. and check ip from jwt with the one we saved to UserLogins
            // todo might also use redis instead of going to db directly
            if (string.IsNullOrWhiteSpace(issuedIp))
            {
                return new GrpcValidateTokenResponse
                {
                    IsValid = false,
                    UserId = "Issued IP address not found."
                };
            }

            var loginInfo = await cache.GetAsync<UserLogin>(CacheKeyGenerator.UserTokenInfoKey(token));
            if (loginInfo == null)
            {
                loginInfo = await repository.GetUserLoginInfo(token);
                if (loginInfo == null)
                {
                    return new GrpcValidateTokenResponse
                    {
                        IsValid = false,
                        UserId = "Login info not found."
                    };
                }

                if (loginInfo.IsLoggedOut)
                {
                    return new GrpcValidateTokenResponse
                    {
                        IsValid = false,
                        UserId = "User is already logged out."
                    };
                }
            }

            if (loginInfo.ExpirationDate < DateTime.UtcNow)
            {
                return new GrpcValidateTokenResponse
                {
                    IsValid = false,
                    UserId = "Login info expired."
                };
            }

            if (clientIp != issuedIp)
            {
                return new GrpcValidateTokenResponse
                {
                    IsValid = false,
                    UserId = "Issued IP address is not valid."
                };
            }

            // refresh cache here again.
            await cache.SetAsync(CacheKeyGenerator.UserTokenInfoKey(token), loginInfo, TimeSpan.FromMinutes(15));
            return new GrpcValidateTokenResponse
            {
                IsValid = true,
                UserId = user
            };
        }
        catch (Exception ex)
        {
            return new GrpcValidateTokenResponse
            {
                IsValid = false,
                UserId = ex.Message
            };
        }
    }
}