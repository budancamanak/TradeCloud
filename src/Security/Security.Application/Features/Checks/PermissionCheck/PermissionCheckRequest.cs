using Common.Core.Models;
using MediatR;

namespace Security.Application.Features.Checks.PermissionCheck;

public class PermissionCheckRequest : BaseCheckRequest<MethodResponse>
{
    public required List<string> Permissions { get; set; }
}