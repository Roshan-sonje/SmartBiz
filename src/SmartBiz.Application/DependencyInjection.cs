using Microsoft.Extensions.DependencyInjection;

namespace SmartBiz.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}