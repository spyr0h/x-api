using XApi.Core.Paging.Ports.Interfaces;
using XApi.Core.Paging.Services;

namespace API.Paging.DependencyInjection;

public static class PagingDependenciesExtensions
{
    public static void AddPagingsDependencies(this IServiceCollection services)
        => services
            .AddSingleton<IPagingService, PagingService>();
}
