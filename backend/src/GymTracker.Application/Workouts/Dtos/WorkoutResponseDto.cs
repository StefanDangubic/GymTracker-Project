using GymTracker.Domain.Enums;

namespace GymTracker.Application.Workouts.Dtos;

public class WorkoutResponseDto
{
    public int Id { get; set; }
    public WorkoutType WorkoutType { get; set; }
    public int DurationMinutes { get; set; }
    public int? CaloriesBurned { get; set; }
    public byte IntensityLevel { get; set; }
    public byte FatigueLevel { get; set; }
    public string? Notes { get; set; }
    public DateTime WorkoutDateUtc { get; set; }
}
