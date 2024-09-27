using Core.Tags.Ports.Interfaces;
using XApi.Core.Tags.Models;

namespace XApi.Core.Tags.Services.Interfaces;

public interface ITagService
{
    public IList<Tag> Autocomplete(TagAutocomplete autocomplete);
}
