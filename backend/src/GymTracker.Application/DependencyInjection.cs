using GymTracker.Application.Auth;
using GymTracker.Application.Workouts;
using Microsoft.Extensions.DependencyInjection;

namespace GymTracker.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<AuthService>();
        services.AddScoped<WorkoutService>();

        return services;
    }
}
