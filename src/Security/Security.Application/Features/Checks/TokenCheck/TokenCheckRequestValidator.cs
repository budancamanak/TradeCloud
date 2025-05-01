using Common.Grpc;

namespace Security.Application.Features.Checks.TokenCheck;

public class TokenCheckRequestValidator : BaseCheckRequestValidator<TokenCheckRequest, ValidateTokenResponse>
{
    public TokenCheckRequestValidator() : base()
    {
    }
}