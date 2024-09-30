using XApi.Adapters.Mysql.Search.Adapters;
using XApi.Core.Search.Ports.Interfaces;
using XApi.Core.Search.Services;

namespace XApi.API.Search.DependencyInjection;

public static class SearchDependenciesExtensions
{
    public static void AddSearchDependencies(this IServiceCollection services)
        => services
            .AddSingleton<ISearchService, SearchService>()
            .AddSingleton<ISearchProvider, SearchProvider>();
}
