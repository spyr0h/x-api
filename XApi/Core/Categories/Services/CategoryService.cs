using XApi.Core.Autocomplete.Helpers;
using XApi.Core.Categories.Models;
using XApi.Core.Categories.Ports.Interfaces;

namespace XApi.Core.Categories.Services;

public class CategoryService(ICategoryProvider categoryProvider) : ICategoryService
{
    public async Task<IList<Category>> Autocomplete(CategoryAutocomplete autocomplete)
    {
        if (string.IsNullOrEmpty(autocomplete.Value)) return [];

        var categories = await categoryProvider.ProvideAllCategories();

        return categories.SearchAndSortByProximity(autocomplete.Value!.ToLower()).ToList();
    }

    public Task<IList<Category>> ProvideAllCategories()
        => categoryProvider.ProvideAllCategories();

    public Task<IList<Category>> ProvideCategoriesForIds(int[] ids)
        => categoryProvider.ProvideCategoriesForIds(ids);

    public async Task<IList<Category>> ProvideCategoriesForTerms(string terms)
    {
        var splittedTerms = terms.Split(" ");
        var tasks = splittedTerms.Select(categoryProvider.ProvideCategoriesForNonCompleteValue);

        return (await Task.WhenAll(tasks))
            .SelectMany(cats => cats)
            .GroupBy(cat => cat.ID)
            .OrderByDescending(group => group.Count())
            .Take(2)
            .Select(group => group.First())
            .ToList();
    }

    public Task<Category?> ProvideCategoryForValue(string value)
        => categoryProvider.ProvideCategoryForValue(value);
}