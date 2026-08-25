using GymTracker.Application.Auth.Dtos;
using GymTracker.Application.Common.Exceptions;
using GymTracker.Application.Common.Interfaces;
using GymTracker.Domain.Entities;

namespace GymTracker.Application.Auth;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<(AuthUserResponseDto User, string Token)> RegisterAsync(
        RegisterRequestDto dto, CancellationToken cancellationToken = default)
    {
        var existingUser = await _userRepository.GetByEmailAsync(dto.Email, cancellationToken);
        if (existingUser is not null)
        {
            throw new ConflictException("A user with this email is already registered.");
        }

        var user = new User
        {
            Email = dto.Email,
            FullName = dto.FullName,
            PasswordHash = _passwordHasher.Hash(dto.Password),
            CreatedAtUtc = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user, cancellationToken);

        var token = _jwtTokenGenerator.GenerateToken(user);
        return (user.ToDto(), token);
    }

    public async Task<(AuthUserResponseDto User, string Token)> LoginAsync(
        LoginRequestDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email, cancellationToken);
        if (user is null || !_passwordHasher.Verify(user.PasswordHash, dto.Password))
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        var token = _jwtTokenGenerator.GenerateToken(user);
        return (user.ToDto(), token);
    }

    public async Task<AuthUserResponseDto> GetCurrentUserAsync(
        int userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        return user.ToDto();
    }
}
