using XApi.Core.Categories.Models;
using XApi.Core.Tags.Models;

namespace XApi.Core.Categories.Ports.Interfaces;

public interface ICategoryService
{
    Task<IList<Category>> ProvideAllCategories();
    Task<IList<Category>> Autocomplete(CategoryAutocomplete autocomplete);
    Task<IList<Category>> ProvideCategoriesForIds(int[] ids);
    Task<Category?> ProvideCategoryForValue(string value);
}