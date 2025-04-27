using Common.Security.Enums;

namespace Common.Security.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class HasPermissionAttribute(Permissions.Enum permission) : Attribute
{
    public Permissions.Enum Permission { get; } = permission;
}