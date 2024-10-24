using XApi.Core.Pornstars.Models;
using XApi.Core.Pornstars.Ports.Interfaces;

namespace XApi.Core.Pornstars.Services;

public class PornstarService(IPornstarProvider pornstarProvider) : IPornstarService
{
    public async Task<IList<Pornstar>> Autocomplete(PornstarAutocomplete autocomplete)
    {
        if (string.IsNullOrEmpty(autocomplete.Value)) return [];

        var pornstars = await pornstarProvider.ProvideAllPornstars();

        return SearchAndSortByProximity(pornstars, autocomplete.Value!.ToLower()).ToList();
    }

    public Task<IList<Pornstar>> ProvideAllPornstars()
        => pornstarProvider.ProvideAllPornstars();

    public Task<Pornstar?> ProvidePornstarForValue(string value)
        => pornstarProvider.ProvidePornstarForValue(value);

    public Task<IList<Pornstar>> ProvidePornstarsForIds(int[] ids)
        => pornstarProvider.ProvidePornstarsForIds(ids);

    private IEnumerable<Pornstar> SearchAndSortByProximity(IEnumerable<Pornstar> pornstars, string fragment)
        => pornstars
            .Where(pornstar => !string.IsNullOrEmpty(pornstar.Value))
            .Select(pornstar => new { Pornstar = pornstar, Index = pornstar.Value!.IndexOf(fragment, StringComparison.OrdinalIgnoreCase) })
            .Where(x => x.Index >= 0)
            .OrderBy(x => x.Index)
            .Select(x => x.Pornstar);
}