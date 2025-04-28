using Common.Core.Models;

namespace Common.Security.Enums;

public sealed class Roles : Enumeration<Roles>
{
    public enum Enum
    {
        Admin,
        Trader,
        Analyst,
        ScriptDeveloper,
        Viewer,
        QA
    }

    public static readonly Roles Admin = new Roles(1, nameof(Admin));
    public static readonly Roles Trader = new Roles(2, nameof(Trader));
    public static readonly Roles Analyst = new Roles(3, nameof(Analyst));
    public static readonly Roles ScriptDeveloper = new Roles(4, nameof(ScriptDeveloper));
    public static readonly Roles Viewer = new Roles(5, nameof(Viewer));
    public static readonly Roles QA = new Roles(6, nameof(QA));

    private Roles(int id, string name) : base(id, name)
    {
    }
}