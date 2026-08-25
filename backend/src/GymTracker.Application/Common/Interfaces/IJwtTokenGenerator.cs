using GymTracker.Domain.Entities;

namespace GymTracker.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
