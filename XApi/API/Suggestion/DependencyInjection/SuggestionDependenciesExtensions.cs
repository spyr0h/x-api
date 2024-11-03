using XApi.Adapters.Mysql.Search.Adapters;
using XApi.Core.Suggestion.Ports.Interfaces;
using XApi.Core.Suggestion.Services;

namespace XApi.API.Suggestion.DependencyInjection;

public static class SuggestionDependenciesExtensions
{
    public static void AddSuggestionDependencies(this IServiceCollection services)
        => services
            .AddSingleton<ISuggestionService, SuggestionService>()
            .AddSingleton<ISuggestionProvider, SuggestionProvider>();
}
