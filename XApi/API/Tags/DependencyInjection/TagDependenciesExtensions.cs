using Core.Tags.Ports.Interfaces;
using XApi.Adapters.Mysql.Tags.Adapters;
using XApi.Core.Tags.Services;
using XApi.Core.Tags.Services.Interfaces;

namespace API.Tags.DependencyInjection;

public static class TagDependenciesExtensions
{
    public static void AddTagsDependencies(this IServiceCollection services)
        => services
            .AddSingleton<ITagService, TagService>()
            .AddSingleton<ITagProvider, TagProvider>();
}
