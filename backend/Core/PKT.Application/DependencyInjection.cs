using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace PKT.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}
