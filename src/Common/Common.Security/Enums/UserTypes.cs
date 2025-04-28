using Common.Core.Models;

namespace Common.Security.Enums;

public sealed class UserTypes : Enumeration<UserTypes>
{
    public static readonly UserTypes Admin = new UserTypes(1, nameof(Admin));
    public static readonly UserTypes Superuser = new UserTypes(2, nameof(Superuser));
    public static readonly UserTypes User = new UserTypes(3, nameof(User));

    private UserTypes(int value, string name) : base(value, name)
    {
    }
}