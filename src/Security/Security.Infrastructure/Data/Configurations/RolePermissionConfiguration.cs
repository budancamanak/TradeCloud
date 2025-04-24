using Common.Security.Enums;
using Microsoft.EntityFrameworkCore;
using Security.Domain.Entities;

namespace Security.Infrastructure.Data.Configurations;

public static class RolePermissionConfiguration
{
    public static void ApplyRolePermissionConfigurations(this ModelBuilder modelBuilder)
    {
        var ent = modelBuilder.Entity<RolePermission>();
        ent.ToTable("RolePermission");
        ent.HasKey(f => new { f.RoleId, f.PermissionId });
        ent.HasData(
            Create(Roles.Admin, Permissions.AssignRoles),
            Create(Roles.Admin, Permissions.ManageScripts),
            Create(Roles.Admin, Permissions.ManageUsers),
            Create(Roles.Admin, Permissions.ManageTrackList),
            Create(Roles.Admin, Permissions.ExecuteTrades),
            Create(Roles.Admin, Permissions.RunAnalysis),
            Create(Roles.Admin, Permissions.ViewResults),
            Create(Roles.Analyst, Permissions.ViewResults),
            Create(Roles.Analyst, Permissions.ViewMarketData),
            Create(Roles.Analyst, Permissions.ManageTrackList),
            Create(Roles.Trader, Permissions.ExecuteTrades),
            Create(Roles.Trader, Permissions.RunAnalysis),
            Create(Roles.Trader, Permissions.ScheduleTask),
            Create(Roles.Trader, Permissions.ViewResults),
            Create(Roles.Viewer, Permissions.ViewMarketData),
            Create(Roles.ScriptDeveloper, Permissions.ViewMarketData),
            Create(Roles.ScriptDeveloper, Permissions.ManageScripts),
            Create(Roles.ScriptDeveloper, Permissions.ScheduleTask),
            Create(Roles.ScriptDeveloper, Permissions.RunAnalysis),
            Create(Roles.ScriptDeveloper, Permissions.ViewResults),
            Create(Roles.QA, Permissions.ViewResults)
        );
    }

    private static RolePermission Create(Roles role, Permissions permission)
    {
        return new RolePermission
        {
            RoleId = role.Value,
            PermissionId = permission.Value
        };
    }
}