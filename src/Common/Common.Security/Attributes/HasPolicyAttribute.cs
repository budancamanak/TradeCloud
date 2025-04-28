namespace Common.Security.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class HasPolicyAttribute(string policy) : Attribute
{
    public string Policy { get; } = policy;
}