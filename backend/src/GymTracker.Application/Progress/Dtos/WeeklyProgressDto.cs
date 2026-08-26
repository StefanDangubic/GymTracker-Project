namespace GymTracker.Application.Progress.Dtos;

public class WeeklyProgressDto
{
    public int WeekNumber { get; set; }
    public DateTime WeekStartDateUtc { get; set; }
    public DateTime WeekEndDateUtc { get; set; }
    public int TotalDurationMinutes { get; set; }
    public int WorkoutCount { get; set; }
    public double? AverageIntensityLevel { get; set; }
    public double? AverageFatigueLevel { get; set; }
}
