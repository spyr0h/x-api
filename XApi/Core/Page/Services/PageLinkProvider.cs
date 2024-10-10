using XApi.Core.Page.Models;
using XApi.Core.Page.Ports.Interfaces;
using XApi.Core.Search.Models;

namespace XApi.Core.Page.Services;

public class PageLinkProvider : IPageLinkProvider
{
    public PageLink ProvidePageLink(SearchCriteria criteria)
    {
        var tagsPart = string.Join('+', criteria.Tags);
        tagsPart = string.IsNullOrEmpty(tagsPart) ? string.Empty : $"tags={tagsPart}";
        var pornstarsPart = string.Join('+', criteria.Pornstars);
        pornstarsPart = string.IsNullOrEmpty(pornstarsPart) ? string.Empty : $"pornstars={pornstarsPart}";

        var parts = new string[] { tagsPart, pornstarsPart }.ToList().Where(part => !string.IsNullOrEmpty(part));

        return new()
        {
            Url = $"/video/search?{string.Join('&', parts)}"
        };
    }
}
