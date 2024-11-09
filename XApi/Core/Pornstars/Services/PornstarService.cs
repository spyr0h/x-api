using XApi.Core.Autocomplete.Helpers;
using XApi.Core.Pornstars.Models;
using XApi.Core.Pornstars.Ports.Interfaces;

namespace XApi.Core.Pornstars.Services;

public class PornstarService(IPornstarProvider pornstarProvider) : IPornstarService
{
    public async Task<IList<Pornstar>> Autocomplete(PornstarAutocomplete autocomplete)
    {
        if (string.IsNullOrEmpty(autocomplete.Value)) return [];

        var pornstars = await pornstarProvider.ProvideAllPornstars();

        return pornstars.SearchAndSortByProximity(autocomplete.Value!.ToLower()).ToList();
    }

    public Task<IList<Pornstar>> ProvideAllPornstars()
        => pornstarProvider.ProvideAllPornstars();

    public Task<Pornstar?> ProvidePornstarForValue(string value)
        => pornstarProvider.ProvidePornstarForValue(value);

    public Task<IList<Pornstar>> ProvidePornstarsForIds(int[] ids)
        => pornstarProvider.ProvidePornstarsForIds(ids);

    public async Task<IList<Pornstar>> ProvidePornstarsForTerms(string terms)
    {
        var splittedTerms = terms.Split(" ");
        var tasks = splittedTerms.Select(pornstarProvider.ProvidePornstarsForNonCompleteValue);

        return (await Task.WhenAll(tasks))
            .SelectMany(pornstars => pornstars)
            .GroupBy(pornstar => pornstar.ID)
            .OrderByDescending(group => group.Count())
            .Take(2)
            .Select(group => group.First())
            .ToList();
    }
}