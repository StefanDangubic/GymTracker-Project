using GymTracker.Domain.Enums;

namespace GymTracker.Domain.Entities;

public class Workout
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public WorkoutType WorkoutType { get; set; }
    public int DurationMinutes { get; set; }
    public int? CaloriesBurned { get; set; }
    public byte IntensityLevel { get; set; }
    public byte FatigueLevel { get; set; }
    public string? Notes { get; set; }
    public DateTime WorkoutDateUtc { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}
