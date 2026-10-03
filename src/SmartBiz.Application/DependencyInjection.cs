using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SmartBiz.Application.Interfaces;
using SmartBiz.Application.Services;


namespace SmartBiz.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register all FluentValidation validators in this assembly
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        // Application services
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}