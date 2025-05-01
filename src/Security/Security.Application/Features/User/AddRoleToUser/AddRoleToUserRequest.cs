using Common.Core.Models;
using MediatR;

namespace Security.Application.Features.User.AddRoleToUser;

public class AddRoleToUserRequest: IRequest<MethodResponse>
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
}