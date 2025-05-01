using Common.Grpc;
using Security.Domain.Entities;

namespace Security.Application.Abstraction.Services;

public interface ITokenService
{
    string GenerateToken(User user,string clientIp);
    Task<GrpcValidateTokenResponse> ValidateToken(string token, string clientIp);
}