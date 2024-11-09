using XApi.Core.Autocomplete.Helpers;
using XApi.Core.Categories.Ports.Interfaces;
using XApi.Core.Tags.Models;
using XApi.Core.Tags.Ports.Interfaces;

namespace XApi.Core.Tags.Services;

public class TagService(ITagProvider tagProvider) : ITagService
{
    public async Task<IList<Tag>> Autocomplete(TagAutocomplete autocomplete)
    {
        if (string.IsNullOrEmpty(autocomplete.Value)) return [];

        var tags = await tagProvider.ProvideAllTags();

        return tags.SearchAndSortByProximity(autocomplete.Value!.ToLower()).ToList();
    }

    public Task<Tag?> ProvideTagForValue(string value)
        => tagProvider.ProvideTagForValue(value);

    public Task<IList<Tag>> ProvideTagsForIds(int[] ids)
        => tagProvider.ProvideTagsForIds(ids);

    public async Task<IList<Tag>> ProvideTagsForTerms(string terms)
    {
        var splittedTerms = terms.Split(" ");
        var tasks = splittedTerms.Select(tagProvider.ProvideTagsForNonCompleteValue);

        return (await Task.WhenAll(tasks))
            .SelectMany(tags => tags)
            .GroupBy(tag => tag.ID)
            .OrderByDescending(group => group.Count())
            .Take(2)
            .Select(group => group.First())
            .ToList();
    }
}
