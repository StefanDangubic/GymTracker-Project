using GymTracker.Application.Auth.Dtos;
using GymTracker.Domain.Entities;

namespace GymTracker.Application.Auth;

public static class AuthMappingExtensions
{
    public static AuthUserResponseDto ToDto(this User user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        FullName = user.FullName
    };
}
