using XApi.Core.Pornstars.Models;

namespace XApi.Core.Pornstars.Ports.Interfaces;

public interface IPornstarService
{
    Task<IList<Pornstar>> ProvideAllPornstars();
    Task<IList<Pornstar>> Autocomplete(PornstarAutocomplete autocomplete);
    Task<Pornstar?> ProvidePornstarForValue(string value);
    Task<IList<Pornstar>> ProvidePornstarsForIds(int[] ids);
    Task<IList<Pornstar>> ProvidePornstarsForTerms(string terms);
}
