using XApi.Adapters.Mysql.Categories.Adapters;
using XApi.Core.Categories.Ports.Interfaces;
using XApi.Core.Categories.Services;

namespace XApi.API.Categories.DependencyInjection;

public static class CategoryDependenciesExtensions
{
    public static void AddCategoriesDependencies(this IServiceCollection services)
        => services
            .AddSingleton<ICategoryService, CategoryService>()
            .AddSingleton<ICategoryProvider, CategoryProvider>();
}
