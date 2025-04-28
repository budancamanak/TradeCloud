using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Common.Grpc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Security.Application.Abstraction.Services;
using Security.Domain.Entities;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;


namespace Security.Infrastructure.Services;

public sealed class TokenService(IConfiguration configuration) : ITokenService
{
    public string GenerateToken(User user, string clientIp)
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

    public async Task<ValidateTokenResponse> ValidateToken(string token, string clientIp)
    {
        // var tokenValidator = new JwtTokenValidator(configuration);
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
            return new ValidateTokenResponse
            {
                IsValid = false,
                UserId = tokenValidationResult.Exception.Message
            };
        }

        if (!tokenValidationResult.ClaimsIdentity.IsAuthenticated)
        {
            return new ValidateTokenResponse
            {
                IsValid = false,
                UserId = "Token is valid but not authenticated"
            };
        }

        var user = tokenValidationResult.ClaimsIdentity.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrWhiteSpace(user))
        {
            return new ValidateTokenResponse
            {
                IsValid = false,
                UserId = "Token is valid but not authenticated"
            };
        }

        var issuedIp = tokenValidationResult.ClaimsIdentity.FindFirst(JwtRegisteredClaimNames.Address)?.Value;
        // todo enabled below. and check ip from jwt with the one we saved to UserLogins
        // todo might also use redis instead of going to db directly
        if (string.IsNullOrWhiteSpace(issuedIp) && false)
        {
            return new ValidateTokenResponse
            {
                IsValid = false,
                UserId = "Issued IP address not found."
            };
        }

        return new ValidateTokenResponse
        {
            IsValid = true,
            UserId = user
        };
    }
}