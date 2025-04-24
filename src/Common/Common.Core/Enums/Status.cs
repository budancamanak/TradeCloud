using Common.Core.Models;

namespace Common.Core.Enums;

public sealed class Status : Enumeration<Status>
{
    public static readonly Status Active = new Status(1, "Active");
    public static readonly Status Passive = new Status(2, "Passive");
    public static readonly Status Removed = new Status(3, "Removed");

    private Status(int value, string name) : base(value, name)
    {
    }
}