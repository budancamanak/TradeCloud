using Common.Core.Models;
using MediatR;

namespace Security.Application.Features.User.AddPermissionToRole;

public class AddPermissionToRoleRequest : IRequest<MethodResponse>
{
    public int RoleId { get; set; }
    public int PermissionId { get; set; }
}