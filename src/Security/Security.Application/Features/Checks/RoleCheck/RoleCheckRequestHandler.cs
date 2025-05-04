using Common.Core.Models;
using Common.Logging.Events.Security;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Security.Application.Abstraction.Services;

namespace Security.Application.Features.Checks.RoleCheck;

public class RoleCheckRequestHandler(
    IValidator<RoleCheckRequest> validator,
    IUserService userService,
    ILogger<RoleCheckRequestHandler> logger,
    ITokenService tokenService)
    : IRequestHandler<RoleCheckRequest, MethodResponse>
{
    public async Task<MethodResponse> Handle(RoleCheckRequest request, CancellationToken cancellationToken)
    {
        var validated = await validator.ValidateAsync(request, cancellationToken);
        if (validated is { IsValid: false })
        {
            return MethodResponse.Error(string.Join(" && ", validated.Errors));
        }

        logger.LogWarning(SecurityChecksLogEvents.RoleCheck,
            "Controlling token[{Token}] for Roles[{Role}] check. IP: {ClientIp}", request.Token,
            string.Join(",", request.Roles), request.ClientIp);

        var valid = await tokenService.ValidateToken(request.Token, request.ClientIp);
        if (!valid.IsValid) return MethodResponse.Error("Token is invalid");
        var userRoles = await userService.GetUserRoles(valid.UserId);
        var hasPermission = userRoles.Any(f => request.Roles.FirstOrDefault(rr => rr == f.Name) != null);
        // var hasPermission = userRoles.Any(f => f.Name == request.Role);
        return new MethodResponse { IsSuccess = hasPermission };
    }
}