namespace GymTracker.Application.Progress.Dtos;

public class MonthlyProgressResponseDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public List<WeeklyProgressDto> Weeks { get; set; } = new();
}
