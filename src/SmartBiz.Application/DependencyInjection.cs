using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SmartBiz.Application.Interfaces;
using SmartBiz.Application.Services;

namespace SmartBiz.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        // Application services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IBusinessService, BusinessService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}