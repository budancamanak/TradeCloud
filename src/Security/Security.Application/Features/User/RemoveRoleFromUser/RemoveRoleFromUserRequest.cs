using Common.Core.Models;
using MediatR;

namespace Security.Application.Features.User.RemoveRoleFromUser;

public class RemoveRoleFromUserRequest : IRequest<MethodResponse>
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
}