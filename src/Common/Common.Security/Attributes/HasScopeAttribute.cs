namespace Common.Security.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class HasScopeAttribute(string scope) : Attribute
{
    public string Scope { get; } = scope;
}