using XApi.Core.Seo.Builders;
using XApi.Core.Seo.Builders.Interfaces;
using XApi.Core.Seo.Services;
using XApi.Core.Seo.Services.Interfaces;

namespace XApi.API.Pornstars.DependencyInjection;

public static class SeoDependenciesExtensions
{
    public static void AddSeoDependencies(this IServiceCollection services)
        => services
            .AddSingleton<ISeoService, SeoService>()
            .AddSingleton<ITitleBuilder, TitleBuilder>()
            .AddSingleton<IDescriptionBuilder, DescriptionBuilder>()
            .AddSingleton<IHeadLineBuilder, HeadlineBuilder>();
}
