using XApi.Adapters.Mysql.Search.Adapters;
using XApi.Core.Videos.Ports.Interfaces;
using XApi.Core.Videos.Services;

namespace XApi.API.Tags.DependencyInjection;

public static class VideoDependenciesExtensions
{
    public static void AddVideoDependencies(this IServiceCollection services)
        => services
            .AddSingleton<IVideoService, VideoService>()
            .AddSingleton<IVideoProvider, VideoProvider>();
}
