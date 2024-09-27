using Mapster;
using MapsterMapper;
using System.Reflection;

namespace API.Tags.DependencyInjection;

public static class MapsterDependenciesExtensions
{
    public static IServiceCollection AddMapster(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;

        config.Scan(Assembly.GetExecutingAssembly());

        config.RequireExplicitMapping = true;

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        return services;
    }
}
