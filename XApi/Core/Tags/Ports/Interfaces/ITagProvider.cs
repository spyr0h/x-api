using XApi.Core.Tags.Models;

namespace Core.Tags.Ports.Interfaces;

public interface ITagProvider
{
    IList<Tag> ProvideAllTags();
}
