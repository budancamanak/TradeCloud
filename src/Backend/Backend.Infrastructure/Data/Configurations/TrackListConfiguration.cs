using Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace Backend.Infrastructure.Data.Configurations;

public static class TrackListConfiguration
{
    public static void ApplyTrackListConfigurations(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TrackList>().ToTable("UserTrackLists");
        modelBuilder.Entity<TrackList>().HasKey(f => new { f.TickerId, f.UserId });
        modelBuilder.Entity<TrackList>().Property(f => f.UserId).IsRequired();
        modelBuilder.Entity<TrackList>().Property(f => f.TickerId).IsRequired();
    }
}