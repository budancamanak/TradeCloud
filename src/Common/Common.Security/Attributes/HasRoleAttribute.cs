using Common.Security.Enums;

namespace Common.Security.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class HasRoleAttribute(Roles.Enum role) : Attribute
{
    public Roles.Enum Role { get; } = role;
}