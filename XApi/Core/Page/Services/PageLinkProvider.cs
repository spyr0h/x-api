using XApi.Core.Page.Models;
using XApi.Core.Page.Ports.Interfaces;
using XApi.Core.Search.Models;

namespace XApi.Core.Page.Services;

public class PageLinkProvider : IPageLinkProvider
{
    public PageLink ProvidePageLink(SearchCriteria criteria)
    {
        var tagsPart = string.Join('+', criteria.Tags.Select(tag => Slugify(tag.Value)));
        tagsPart = string.IsNullOrEmpty(tagsPart) ? string.Empty : $"tags={tagsPart}";
        var pornstarsPart = string.Join('+', criteria.Pornstars.Select(pornstar => Slugify(pornstar.Value)));
        pornstarsPart = string.IsNullOrEmpty(pornstarsPart) ? string.Empty : $"pornstars={pornstarsPart}";

        var page = criteria.Paging.PageIndex == 1 ? null : $"page={criteria.Paging.PageIndex}";

        var parts = new string[] { tagsPart, pornstarsPart, page }.ToList().Where(part => !string.IsNullOrEmpty(part));

        return new()
        {
            Url = $"/video/search?{string.Join('&', parts)}"
        };
    }

    private string Slugify(string value)
    {
        if (value == null) return "error";
        var splitted = value.Split(' ').Select(split => split.ToLower());
        return string.Join("%20", splitted);
    }
}
