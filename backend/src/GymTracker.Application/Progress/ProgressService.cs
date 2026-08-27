using System.Globalization;
using GymTracker.Application.Common.Interfaces;
using GymTracker.Application.Progress.Dtos;

namespace GymTracker.Application.Progress;

public class ProgressService
{
    private readonly IWorkoutRepository _workoutRepository;

    public ProgressService(IWorkoutRepository workoutRepository)
    {
        _workoutRepository = workoutRepository;
    }

    // Weeks follow the ISO-8601 definition (Monday-Sunday). A week that straddles a month
    // boundary is still reported once for the selected month, but its aggregates only
    // include the days that actually fall within that month.
    public async Task<MonthlyProgressResponseDto> GetMonthlyProgressAsync(
        int userId, int year, int month, CancellationToken cancellationToken = default)
    {
        var monthStart = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        var monthWorkouts = await _workoutRepository.GetByUserIdAndDateRangeAsync(
            userId, monthStart, monthEnd, cancellationToken);

        var weekKeys = new List<(int IsoYear, int IsoWeek)>();
        for (var day = monthStart; day <= monthEnd; day = day.AddDays(1))
        {
            var key = (ISOWeek.GetYear(day), ISOWeek.GetWeekOfYear(day));
            if (!weekKeys.Contains(key))
            {
                weekKeys.Add(key);
            }
        }

        var weeks = weekKeys.Select(key =>
        {
            // ISOWeek.ToDateTime returns DateTimeKind.Unspecified, which System.Text.Json
            // serializes without a trailing "Z" - explicitly mark it Utc so week boundaries
            // serialize consistently with every other UTC timestamp in the API.
            var weekStart = DateTime.SpecifyKind(
                ISOWeek.ToDateTime(key.IsoYear, key.IsoWeek, DayOfWeek.Monday), DateTimeKind.Utc);
            var weekWorkouts = monthWorkouts
                .Where(w => ISOWeek.GetYear(w.WorkoutDateUtc) == key.IsoYear
                            && ISOWeek.GetWeekOfYear(w.WorkoutDateUtc) == key.IsoWeek)
                .ToList();

            return new WeeklyProgressDto
            {
                WeekNumber = key.IsoWeek,
                WeekStartDateUtc = weekStart,
                WeekEndDateUtc = weekStart.AddDays(6),
                TotalDurationMinutes = weekWorkouts.Sum(w => w.DurationMinutes),
                WorkoutCount = weekWorkouts.Count,
                AverageIntensityLevel = weekWorkouts.Count > 0
                    ? Math.Round(weekWorkouts.Average(w => w.IntensityLevel), 2)
                    : null,
                AverageFatigueLevel = weekWorkouts.Count > 0
                    ? Math.Round(weekWorkouts.Average(w => w.FatigueLevel), 2)
                    : null
            };
        })
        .OrderBy(w => w.WeekStartDateUtc)
        .ToList();

        return new MonthlyProgressResponseDto
        {
            Year = year,
            Month = month,
            Weeks = weeks
        };
    }
}
