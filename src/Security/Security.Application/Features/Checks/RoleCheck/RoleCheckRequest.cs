using Common.Core.Models;
using MediatR;

namespace Security.Application.Features.Checks.RoleCheck;

public class RoleCheckRequest : BaseCheckRequest, IRequest<MethodResponse>
{
    public required string Role { get; set; }
}