namespace GymTracker.Application.Auth.Dtos;

public class AuthUserResponseDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
}
