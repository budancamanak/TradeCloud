using Microsoft.Extensions.Logging;

namespace Common.Logging.Events.Security;

public static class SecurityChecksLogEvents
{
    public static readonly EventId PermissionCheck = new EventId(1, "PermissionCheckRequest");
    public static readonly EventId RoleCheck = new EventId(2, "RoleCheckRequest");
    public static readonly EventId TokenCheck = new EventId(3, "TokenCheckRequest");
}