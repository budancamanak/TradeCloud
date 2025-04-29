using Common.Core.Models;
using MediatR;

namespace Security.Application.Features.Checks.PermissionCheck;

public class PermissionCheckRequestHandler : IRequestHandler<PermissionCheckRequest, MethodResponse>
{
    public Task<MethodResponse> Handle(PermissionCheckRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}