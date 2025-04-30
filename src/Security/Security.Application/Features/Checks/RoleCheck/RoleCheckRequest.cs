using Common.Core.Models;
using MediatR;

namespace Security.Application.Features.Checks.RoleCheck;

public class RoleCheckRequest : BaseCheckRequest<MethodResponse>
{
    public required List<string> Roles { get; set; }
}