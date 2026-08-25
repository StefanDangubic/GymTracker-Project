using GymTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymTracker.Infrastructure.Persistence.Configurations;

public class WorkoutConfiguration : IEntityTypeConfiguration<Workout>
{
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
            .IsRequired();

        builder.Property(w => w.CreatedAtUtc)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.HasOne(w => w.User)
            .WithMany(u => u.Workouts)
            .HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(w => new { w.UserId, w.WorkoutDateUtc });
    }
}
