using Common.Grpc;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Security.Application.Abstraction.Services;

namespace Security.Application.Features.Checks;

public class TokenCheckRequestHandler(
    ILogger<TokenCheckRequestHandler> logger,
    ITokenService tokenService,
    IValidator<BaseCheckRequest<ValidateTokenResponse>> validator)
    : IRequestHandler<BaseCheckRequest<ValidateTokenResponse>, ValidateTokenResponse>
{
    public async Task<ValidateTokenResponse> Handle(BaseCheckRequest<ValidateTokenResponse> request,
        CancellationToken cancellationToken)
    {
        var validated = await validator.ValidateAsync(request, cancellationToken);
        if (validated is { IsValid: false })
        {
            return new ValidateTokenResponse
            {
                IsValid = validated.IsValid,
                UserId = string.Join(" && ", validated.Errors)
            };
        }

        logger.LogWarning("Controlling token for role check. IP: {ClientIp}", request.ClientIp);
        return await tokenService.ValidateToken(request.Token, request.ClientIp);
    }
}