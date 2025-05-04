using Common.Grpc;
using Common.Logging.Events.Security;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Security.Application.Abstraction.Services;

namespace Security.Application.Features.Checks.TokenCheck;

public class TokenCheckRequestHandler(
    ILogger<TokenCheckRequestHandler> logger,
    ITokenService tokenService,
    IValidator<TokenCheckRequest> validator)
    : IRequestHandler<TokenCheckRequest, GrpcValidateTokenResponse>
{
    public async Task<GrpcValidateTokenResponse> Handle(TokenCheckRequest request,
        CancellationToken cancellationToken)
    {
        var validated = await validator.ValidateAsync(request, cancellationToken);
        if (validated is { IsValid: false })
        {
            return new GrpcValidateTokenResponse
            {
                IsValid = validated.IsValid,
                UserId = string.Join(" && ", validated.Errors)
            };
        }

        logger.LogWarning(SecurityChecksLogEvents.TokenCheck,
            "Validating token[{Token}]. IP: {ClientIp}", request.Token, request.ClientIp);
        return await tokenService.ValidateToken(request.Token, request.ClientIp);
    }
}