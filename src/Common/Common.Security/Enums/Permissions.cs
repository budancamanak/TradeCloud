using Common.Core.Models;

namespace Common.Security.Enums;

public sealed class Permissions : Enumeration<Permissions>
{
    public enum Enum
    {
        ViewMarketData,
        RunAnalysis,
        ScheduleTask,
        ManageScripts,
        ExecuteTrades,
        ViewResults,
        ManageUsers,
        AssignRoles,
        ManageTrackList
    }

    public static readonly Permissions ViewMarketData = new Permissions(1, nameof(ViewMarketData));
    public static readonly Permissions RunAnalysis = new Permissions(2, nameof(RunAnalysis));
    public static readonly Permissions ScheduleTask = new Permissions(3, nameof(ScheduleTask));
    public static readonly Permissions ManageScripts = new Permissions(4, nameof(ManageScripts));
    public static readonly Permissions ExecuteTrades = new Permissions(5, nameof(ExecuteTrades));
    public static readonly Permissions ViewResults = new Permissions(6, nameof(ViewResults));
    public static readonly Permissions ManageUsers = new Permissions(7, nameof(ManageUsers));
    public static readonly Permissions AssignRoles = new Permissions(8, nameof(AssignRoles));
    public static readonly Permissions ManageTrackList = new Permissions(9, nameof(ManageTrackList));

    private Permissions(int value, string name) : base(value, name)
    {
    }
}