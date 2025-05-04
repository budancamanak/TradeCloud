using Microsoft.Extensions.Logging;

namespace Common.Logging.Events.Security;

public static class UserLogEvents
{
    public static readonly EventId AddPermissionToRole = new EventId(1, "AddPermissionToRoleRequest");
    public static readonly EventId AddRoleToUser = new EventId(2, "AddRoleToUserRequest");
    public static readonly EventId LoginUser = new EventId(3, "LoginUserRequest");
    public static readonly EventId LogoutUser = new EventId(4, "LogoutUserRequest");
    public static readonly EventId RegisterUser = new EventId(5, "RegisterUserRequest");
    public static readonly EventId RemovePermissionFromRole = new EventId(6, "RemovePermissionFromRoleRequest");
    public static readonly EventId RemoveRoleFromUser = new EventId(7, "RemoveRoleFromUserRequest");
    public static readonly EventId UserService = new EventId(8, "UserService");
    public static readonly EventId TokenService = new EventId(9, "TokenService");
}