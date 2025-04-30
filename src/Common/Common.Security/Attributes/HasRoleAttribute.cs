using Common.Security.Enums;

namespace Common.Security.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class HasRoleAttribute(params Roles.Enum[] roles) : Attribute
{
    public Roles.Enum[] Roles { get; } = roles;
}