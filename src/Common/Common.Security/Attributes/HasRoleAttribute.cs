namespace Common.Security.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class HasRoleAttribute(string role) : Attribute
{
    public string Role { get; } = role;
}