using Common.Grpc;

namespace Security.Application.Features.Checks.TokenCheck;

public class TokenCheckRequest : BaseCheckRequest<GrpcValidateTokenResponse>
{
}