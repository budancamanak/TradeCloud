using Common.Security.Enums;

namespace Common.Security.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class HasPermissionAttribute(params Permissions.Enum[] permissions) : Attribute
{
    public Permissions.Enum[] Permissions { get; } = permissions;
}