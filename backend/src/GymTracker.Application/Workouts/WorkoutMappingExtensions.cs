using GymTracker.Application.Workouts.Dtos;
using GymTracker.Domain.Entities;

namespace GymTracker.Application.Workouts;

public static class WorkoutMappingExtensions
{
    public static WorkoutResponseDto ToDto(this Workout workout) => new()
    {
        Id = workout.Id,
        WorkoutType = workout.WorkoutType,
        DurationMinutes = workout.DurationMinutes,
        CaloriesBurned = workout.CaloriesBurned,
        IntensityLevel = workout.IntensityLevel,
        FatigueLevel = workout.FatigueLevel,
        Notes = workout.Notes,
        WorkoutDateUtc = workout.WorkoutDateUtc
    };
}
