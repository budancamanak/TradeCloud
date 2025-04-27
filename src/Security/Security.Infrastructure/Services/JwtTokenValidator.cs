using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Security.Infrastructure.Services;

public class JwtTokenValidator(IConfiguration configuration)
{
    public bool CanValidateToken => true;
    public bool CanReadToken(string securityToken) => true;
    public int MaximumTokenSizeInBytes { get; set; }

    public ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validationParameters,
        out SecurityToken validatedToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!))
        };

        var claimsPrincipal = handler.ValidateToken(token, tokenValidationParameters, out validatedToken);
        return claimsPrincipal;
    }

    public async Task<ClaimsPrincipal> ValidateTokenAsync(string token, TokenValidationParameters validationParameters)
    {
        var handler = new JwtSecurityTokenHandler();
        // var tokenValidationParameters = new TokenValidationParameters
        // {
        //     ValidateIssuer = true,
        //     ValidateAudience = true,
        //     ValidateLifetime = true,
        //     ValidateIssuerSigningKey = true,
        //     ValidIssuer = configuration["Jwt:Issuer"],
        //     ValidAudience = configuration["Jwt:Audience"],
        //     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!))
        // };

        var tokenValidationResult = await handler.ValidateTokenAsync(token, validationParameters);
        var claimsPrincipal = handler.ValidateToken(token, validationParameters, out SecurityToken securityToken);
        return claimsPrincipal;
    }
}