using XApi.Core.Tags.Models;

namespace Core.Tags.Ports.Interfaces;

public interface ITagProvider
{
    Task<IList<Tag>> ProvideAllTags();
}
