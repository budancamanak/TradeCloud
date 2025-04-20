namespace Common.Web.Attributes.Security;

[Flags]
public enum Permission
{
    ExecuteAll = 1,
    RunAnalysis = 2
}