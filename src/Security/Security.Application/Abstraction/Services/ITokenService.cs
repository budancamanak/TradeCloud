using Security.Domain.Entities;

namespace Security.Application.Abstraction.Services;

public interface ITokenService
{
    string GenerateToken(User user);
}