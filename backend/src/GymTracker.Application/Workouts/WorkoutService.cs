using GymTracker.Application.Common.Exceptions;
using GymTracker.Application.Common.Interfaces;
using GymTracker.Application.Workouts.Dtos;
using GymTracker.Domain.Entities;

namespace GymTracker.Application.Workouts;

public class WorkoutService
{
    private readonly IWorkoutRepository _workoutRepository;

    public WorkoutService(IWorkoutRepository workoutRepository)
    {
        _workoutRepository = workoutRepository;
    }

    public async Task<List<WorkoutResponseDto>> GetUserWorkoutsAsync(
        int userId, CancellationToken cancellationToken = default)
    {
        var workouts = await _workoutRepository.GetByUserIdAsync(userId, cancellationToken);
        return workouts.Select(w => w.ToDto()).ToList();
    }

    public async Task<WorkoutResponseDto> GetByIdAsync(
        int workoutId, int userId, CancellationToken cancellationToken = default)
    {
        var workout = await GetOwnedWorkoutAsync(workoutId, userId, cancellationToken);
        return workout.ToDto();
    }

    public async Task<WorkoutResponseDto> CreateAsync(
        int userId, CreateWorkoutDto dto, CancellationToken cancellationToken = default)
    {
        var workout = new Workout
        {
            UserId = userId,
            WorkoutType = dto.WorkoutType,
            DurationMinutes = dto.DurationMinutes,
            CaloriesBurned = dto.CaloriesBurned,
            IntensityLevel = dto.IntensityLevel,
            FatigueLevel = dto.FatigueLevel,
            Notes = dto.Notes,
            WorkoutDateUtc = dto.WorkoutDateUtc,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _workoutRepository.AddAsync(workout, cancellationToken);
        return workout.ToDto();
    }

    public async Task<WorkoutResponseDto> UpdateAsync(
        int workoutId, int userId, UpdateWorkoutDto dto, CancellationToken cancellationToken = default)
    {
        var workout = await GetOwnedWorkoutAsync(workoutId, userId, cancellationToken);

        workout.WorkoutType = dto.WorkoutType;
        workout.DurationMinutes = dto.DurationMinutes;
        workout.CaloriesBurned = dto.CaloriesBurned;
        workout.IntensityLevel = dto.IntensityLevel;
        workout.FatigueLevel = dto.FatigueLevel;
        workout.Notes = dto.Notes;
        workout.WorkoutDateUtc = dto.WorkoutDateUtc;
        workout.UpdatedAtUtc = DateTime.UtcNow;

        await _workoutRepository.UpdateAsync(workout, cancellationToken);
        return workout.ToDto();
    }

    public async Task DeleteAsync(
        int workoutId, int userId, CancellationToken cancellationToken = default)
    {
        var workout = await GetOwnedWorkoutAsync(workoutId, userId, cancellationToken);
        await _workoutRepository.DeleteAsync(workout, cancellationToken);
    }

    // Ownership is enforced here for every read/write path that targets a single workout:
    // a workout that exists but belongs to another user is treated as not found (404, not 403)
    // so its existence is never revealed to a user who doesn't own it.
    private async Task<Workout> GetOwnedWorkoutAsync(
        int workoutId, int userId, CancellationToken cancellationToken)
    {
        var workout = await _workoutRepository.GetByIdAsync(workoutId, cancellationToken);
        if (workout is null || workout.UserId != userId)
        {
            throw new NotFoundException("Workout not found.");
        }

        return workout;
    }
}
