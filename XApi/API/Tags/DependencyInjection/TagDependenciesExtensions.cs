using XApi.Adapters.Mysql.Tags.Adapters;
using XApi.Core.Tags.Ports.Interfaces;
using XApi.Core.Tags.Services;

namespace XApi.API.Tags.DependencyInjection;

public static class TagDependenciesExtensions
{
    public static void AddTagsDependencies(this IServiceCollection services)
        => services
            .AddSingleton<ITagService, TagService>()
            .AddSingleton<ITagProvider, TagProvider>();
}
