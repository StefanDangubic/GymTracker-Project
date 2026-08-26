using GymTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GymTracker.Infrastructure.Persistence.Configurations;

public class WorkoutConfiguration : IEntityTypeConfiguration<Workout>
{
    // SQL Server's datetime2 has no timezone/Kind concept, so EF Core reads every
    // DateTime back as Kind=Unspecified regardless of what was written. Without this,
    // System.Text.Json omits the "Z" suffix on read (but not on the in-memory value
    // returned right after a write), so POST/PUT and GET responses for the same
    // instant serialize inconsistently and get misread as local time by clients.
    private static readonly ValueConverter<DateTime, DateTime> UtcDateTimeConverter = new(
        v => v,
        v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

    private static readonly ValueConverter<DateTime?, DateTime?> UtcNullableDateTimeConverter = new(
        v => v,
        v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

    public void Configure(EntityTypeBuilder<Workout> builder)
    {
        builder.ToTable("Workouts", t =>
        {
            t.HasCheckConstraint("CK_Workouts_DurationMinutes", "[DurationMinutes] > 0");
            t.HasCheckConstraint("CK_Workouts_CaloriesBurned", "[CaloriesBurned] IS NULL OR [CaloriesBurned] >= 0");
            t.HasCheckConstraint("CK_Workouts_IntensityLevel", "[IntensityLevel] BETWEEN 1 AND 10");
            t.HasCheckConstraint("CK_Workouts_FatigueLevel", "[FatigueLevel] BETWEEN 1 AND 10");
        });

        builder.HasKey(w => w.Id);

        builder.Property(w => w.WorkoutType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(w => w.DurationMinutes)
            .IsRequired();

        builder.Property(w => w.CaloriesBurned);

        builder.Property(w => w.IntensityLevel)
            .IsRequired();

        builder.Property(w => w.FatigueLevel)
            .IsRequired();

        builder.Property(w => w.Notes)
            .HasMaxLength(1000);

        builder.Property(w => w.WorkoutDateUtc)
            .IsRequired()
            .HasConversion(UtcDateTimeConverter);

        builder.Property(w => w.CreatedAtUtc)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()")
            .HasConversion(UtcDateTimeConverter);

        builder.Property(w => w.UpdatedAtUtc)
            .HasConversion(UtcNullableDateTimeConverter);

        builder.HasOne(w => w.User)
            .WithMany(u => u.Workouts)
            .HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(w => new { w.UserId, w.WorkoutDateUtc });
    }
}
