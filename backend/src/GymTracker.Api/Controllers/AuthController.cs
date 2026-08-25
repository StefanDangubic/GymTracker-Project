using GymTracker.Api.Common;
using GymTracker.Application.Auth;
using GymTracker.Application.Auth.Dtos;
using GymTracker.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GymTracker.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private const string AccessTokenCookieName = "access_token";

    private readonly AuthService _authService;
    private readonly IWebHostEnvironment _environment;
    private readonly JwtSettings _jwtSettings;

    public AuthController(AuthService authService, IWebHostEnvironment environment, IOptions<JwtSettings> jwtOptions)
    {
        _authService = authService;
        _environment = environment;
        _jwtSettings = jwtOptions.Value;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthUserResponseDto>> Register(RegisterRequestDto dto, CancellationToken cancellationToken)
    {
        var (user, token) = await _authService.RegisterAsync(dto, cancellationToken);
        SetAuthCookie(token);
        return StatusCode(StatusCodes.Status201Created, user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthUserResponseDto>> Login(LoginRequestDto dto, CancellationToken cancellationToken)
    {
        var (user, token) = await _authService.LoginAsync(dto, cancellationToken);
        SetAuthCookie(token);
        return Ok(user);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(AccessTokenCookieName, new CookieOptions { Path = "/" });
        return Ok();
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<AuthUserResponseDto>> Me(CancellationToken cancellationToken)
    {
        var user = await _authService.GetCurrentUserAsync(User.GetUserId(), cancellationToken);
        return Ok(user);
    }

    private void SetAuthCookie(string token)
    {
        Response.Cookies.Append(AccessTokenCookieName, token, new CookieOptions
        {
            HttpOnly = true,
            Secure = !_environment.IsDevelopment(),
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddHours(_jwtSettings.ExpiryHours),
            Path = "/"
        });
    }
}
