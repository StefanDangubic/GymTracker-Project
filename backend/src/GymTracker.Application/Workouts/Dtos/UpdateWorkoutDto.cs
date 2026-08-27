using System.ComponentModel.DataAnnotations;
using GymTracker.Domain.Enums;

namespace GymTracker.Application.Workouts.Dtos;

public class UpdateWorkoutDto
{
    [Required]
    public WorkoutType? WorkoutType { get; set; }

    [Range(1, int.MaxValue)]
    public int DurationMinutes { get; set; }

    [Range(0, int.MaxValue)]
    public int? CaloriesBurned { get; set; }

    [Range(1, 10)]
    public byte IntensityLevel { get; set; }

    [Range(1, 10)]
    public byte FatigueLevel { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    [Required]
    public DateTime? WorkoutDateUtc { get; set; }
}
