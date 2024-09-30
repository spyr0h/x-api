using Mapster;
using MapsterMapper;
using System.Reflection;
using XApi.Adapters.Mysql.Pornstars.Adapters;
using XApi.Adapters.Mysql.Tags.Adapters;

namespace XApi.API.DependencyInjection;

public static class MapsterDependenciesExtensions
{
    public static IServiceCollection AddMapster(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;

        config.Scan(Assembly.GetExecutingAssembly());

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        var tagProviderToImport = typeof(TagProvider);
        var pornstarProviderToImport = typeof(PornstarProvider);

        assemblies.ToList().ForEach(assembly => config.Scan(assembly));

        config.RequireExplicitMapping = true;

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        return services;
    }
}
