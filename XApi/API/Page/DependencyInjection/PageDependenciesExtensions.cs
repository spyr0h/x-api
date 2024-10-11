using XApi.Core.Page.Ports.Interfaces;
using XApi.Core.Page.Services;

namespace API.Page.DependencyInjection;

public static class PageDependenciesExtensions
{
    public static void AddPagesDependencies(this IServiceCollection services)
        => services
            .AddSingleton<IPageRoutingService, PageRoutingService>()
            .AddSingleton<IPageLinkProvider, PageLinkProvider>();
}
