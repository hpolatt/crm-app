using Microsoft.Extensions.DependencyInjection;

namespace PKT.Mapper;

public static class DependencyInjection
{
    public static IServiceCollection AddMapper(this IServiceCollection services)
    {
        // AutoMapper
        services.AddAutoMapper(typeof(MappingProfile).Assembly);

        return services;
    }
}
