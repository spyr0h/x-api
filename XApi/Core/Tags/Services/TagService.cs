using Core.Tags.Ports.Interfaces;
using XApi.Core.Tags.Models;
using XApi.Core.Tags.Services.Interfaces;

namespace XApi.Core.Tags.Services;

public class TagService(ITagProvider tagProvider) : ITagService
{
    public async Task<IList<Tag>> Autocomplete(TagAutocomplete autocomplete)
    {
        if (string.IsNullOrEmpty(autocomplete.Value)) return [];

        var tags = await tagProvider.ProvideAllTags();

        return SearchAndSortByProximity(tags, autocomplete.Value!.ToLower()).ToList();
    }

    private IEnumerable<Tag> SearchAndSortByProximity(IEnumerable<Tag> tags, string fragment)
        => tags
            .Where(tag => !string.IsNullOrEmpty(tag.Value))
            .Select(tag => new { Tag = tag, Index = tag.Value!.IndexOf(fragment, StringComparison.OrdinalIgnoreCase) })
            .Where(x => x.Index >= 0)
            .OrderBy(x => x.Index)
            .Select(x => x.Tag);
}
