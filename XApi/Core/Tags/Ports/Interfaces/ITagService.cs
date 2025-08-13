using XApi.Core.Categories.Models;
using XApi.Core.Tags.Models;

namespace XApi.Core.Tags.Ports.Interfaces;

public interface ITagService
{
    Task<IList<Tag>> ProvideAllTags();
    public Task<IList<Tag>> Autocomplete(TagAutocomplete autocomplete);
    Task<IList<Tag>> ProvideTagsForIds(int[] ids);
    Task<Tag?> ProvideTagForValue(string value);
    Task<IList<Tag>> ProvideTagsForTerms(string terms);
}
