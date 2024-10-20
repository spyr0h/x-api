using XApi.Core.Categories.Models;

namespace XApi.Core.Categories.Ports.Interfaces;

public interface ICategoryProvider
{
    Task<IList<Category>> ProvideAllCategories();
    Task<Category?> ProvideCategoryForValue(string value);
    Task<IList<Category>> ProvideCategoriesForIds(int[] ids);
}