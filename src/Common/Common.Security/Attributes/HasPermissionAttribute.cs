namespace Common.Security.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class HasPermissionAttribute(string permission) : Attribute
{
    public string Permission { get; } = permission;
}