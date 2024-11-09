using XApi.Core.Autocomplete.Interfaces;
using XApi.Core.Pornstars.Models;

namespace XApi.Core.Autocomplete.Helpers;

public static class AutocompleteExtensions
{
    public static IEnumerable<T> SearchAndSortByProximity<T>(this IEnumerable<T> elements, string fragment) 
        where T : IAutocompletable
        => elements
            .Where(element => !string.IsNullOrEmpty(element.Value))
            .Select(element => new { Element = element, Index = element.Value!.IndexOf(fragment, StringComparison.OrdinalIgnoreCase) })
            .Where(x => x.Index >= 0)
            .OrderBy(x => x.Index)
            .Select(x => x.Element);
}
