using XApi.Core.Linkbox.Ports.Interfaces;
using XApi.Core.Linkbox.Services;

namespace XApi.API.Linkbox.DependencyInjection;

public static class LinkboxDependenciesExtensions
{
    public static void AddLinkboxesDependencies(this IServiceCollection services)
        => services
            .AddSingleton<ILinkboxService, LinkboxService>();
}
