using XApi.Adapters.Mysql.Pornstars.Adapters;
using XApi.Core.Pornstars.Ports.Interfaces;
using XApi.Core.Pornstars.Services;

namespace XApi.API.Pornstars.DependencyInjection;

public static class PornstarDependenciesExtensions
{
    public static void AddPornstarsDependencies(this IServiceCollection services)
        => services
            .AddSingleton<IPornstarService, PornstarService>()
            .AddSingleton<IPornstarProvider, PornstarProvider>();
}
