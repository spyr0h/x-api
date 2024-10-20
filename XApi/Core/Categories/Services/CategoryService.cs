using XApi.Core.Categories.Models;
using XApi.Core.Categories.Ports.Interfaces;

namespace XApi.Core.Categories.Services;

public class CategoryService(ICategoryProvider categoryProvider) : ICategoryService
{
    public async Task<IList<Category>> Autocomplete(CategoryAutocomplete autocomplete)
    {
        if (string.IsNullOrEmpty(autocomplete.Value)) return [];

        var tags = await categoryProvider.ProvideAllCategories();

        return SearchAndSortByProximity(tags, autocomplete.Value!.ToLower()).ToList();
    }

    public Task<IList<Category>> ProvideCategoriesForIds(int[] ids)
        => categoryProvider.ProvideCategoriesForIds(ids);

    public Task<Category?> ProvideCategoryForValue(string value)
        => categoryProvider.ProvideCategoryForValue(value);

    private IEnumerable<Category> SearchAndSortByProximity(IEnumerable<Category> categories, string fragment)
        => categories
            .Where(cat => !string.IsNullOrEmpty(cat.Value))
            .Select(cat => new { Category = cat, Index = cat.Value!.IndexOf(fragment, StringComparison.OrdinalIgnoreCase) })
            .Where(x => x.Index >= 0)
            .OrderBy(x => x.Index)
            .Select(x => x.Category);
}