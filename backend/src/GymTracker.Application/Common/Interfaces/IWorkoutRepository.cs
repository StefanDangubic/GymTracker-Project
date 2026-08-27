using GymTracker.Domain.Entities;

namespace GymTracker.Application.Common.Interfaces;

public interface IWorkoutRepository
{
    Task<Workout?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Workout>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<List<Workout>> GetByUserIdAndDateRangeAsync(
        int userId, DateTime startDateUtc, DateTime endDateUtc, CancellationToken cancellationToken = default);
    Task AddAsync(Workout workout, CancellationToken cancellationToken = default);
    Task UpdateAsync(Workout workout, CancellationToken cancellationToken = default);
    Task DeleteAsync(Workout workout, CancellationToken cancellationToken = default);
}
