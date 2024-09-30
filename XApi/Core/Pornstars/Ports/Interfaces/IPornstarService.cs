using XApi.Core.Pornstars.Models;

namespace XApi.Core.Pornstars.Ports.Interfaces;

public interface IPornstarService
{
    public Task<IList<Pornstar>> Autocomplete(PornstarAutocomplete autocomplete);
}
