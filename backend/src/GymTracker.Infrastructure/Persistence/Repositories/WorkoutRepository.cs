using GymTracker.Application.Common.Interfaces;
using GymTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Infrastructure.Persistence.Repositories;

public class WorkoutRepository : IWorkoutRepository
{
    private readonly AppDbContext _context;

    public WorkoutRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Workout?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _context.Workouts.FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

    public Task<List<Workout>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default) =>
        _context.Workouts
            .Where(w => w.UserId == userId)
            .OrderByDescending(w => w.WorkoutDateUtc)
            .ToListAsync(cancellationToken);

    public Task<List<Workout>> GetByUserIdAndDateRangeAsync(
        int userId, DateTime startDateUtc, DateTime endDateUtc, CancellationToken cancellationToken = default) =>
        _context.Workouts
            .Where(w => w.UserId == userId
                        && w.WorkoutDateUtc.Date >= startDateUtc.Date
                        && w.WorkoutDateUtc.Date <= endDateUtc.Date)
            .OrderByDescending(w => w.WorkoutDateUtc)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Workout workout, CancellationToken cancellationToken = default)
    {
        _context.Workouts.Add(workout);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Workout workout, CancellationToken cancellationToken = default)
    {
        _context.Workouts.Update(workout);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Workout workout, CancellationToken cancellationToken = default)
    {
        _context.Workouts.Remove(workout);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
