using XApi.Core.Tags.Models;

namespace XApi.Core.Tags.Ports.Interfaces;

public interface ITagProvider
{
    Task<IList<Tag>> ProvideAllTags();
    Task<Tag?> ProvideTagForValue(string value);
    Task<IList<Tag>> ProvideTagsForIds(int[] ids);
}
