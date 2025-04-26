using Security.Application.Abstraction.Services;

namespace Security.Infrastructure.Services;

public class TokenService : ITokenService
{
    public string GenerateToken()
    {
        return Guid.NewGuid().ToString().Replace("-", "");
    }
}