using XApi.Core.Tags.Models;

namespace XApi.Core.Tags.Ports.Interfaces;

public interface ITagService
{
    public Task<IList<Tag>> Autocomplete(TagAutocomplete autocomplete);
}
