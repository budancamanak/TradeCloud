using Common.Core.Models;
using MediatR;

namespace Security.Application.Features.User.RemovePermissionFromRole;

public class RemovePermissionFromRoleRequest : IRequest<MethodResponse>
{
    public int RoleId { get; set; }
    public int PermissionId { get; set; }
}