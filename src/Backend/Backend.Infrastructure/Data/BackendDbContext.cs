﻿using Backend.Domain.Entities;
using Backend.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Data;

public class BackendDbContext : DbContext
{
    public DbSet<SystemSetting> SystemSettings { get; set; }
    public DbSet<TrackList> TrackLists { get; set; }
    public DbSet<PluginOutput> PluginOutputs { get; set; }
    public DbSet<PluginExecution> PluginExecutions { get; set; }

    public BackendDbContext(DbContextOptions<BackendDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyPluginExecutionConfigurations();
        modelBuilder.ApplyPluginOutputConfigurations();
        modelBuilder.ApplySystemSettingConfigurations();
        modelBuilder.ApplyTrackListConfigurations();
    }
}