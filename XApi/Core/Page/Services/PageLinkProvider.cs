using XApi.Core.Categories.Models;
using XApi.Core.Page.Models;
using XApi.Core.Page.Ports.Interfaces;
using XApi.Core.Pornstars.Models;
using XApi.Core.Search.Enums;
using XApi.Core.Search.Models;
using XApi.Core.Tags.Models;

namespace XApi.Core.Page.Services;

public class PageLinkProvider : IPageLinkProvider
{
    public PageLink? ProvidePageLink(SearchCriteria criteria)
    {
        var pageLink = criteria switch
        {
            { Terms: not null } => new PageLink { Url = $"/videos/search?terms={string.Join("+", criteria.Terms.Split(" "))}" },
            { Pornstars: [], Categories: [] } when criteria.Tags.Count == 1 => GenerateTagUrl(criteria.Tags),
            { Pornstars: [], Tags: [] } when criteria.Categories.Count == 1 => GenerateCategoryUrl(criteria.Categories),
            { Categories: [], Tags: [] } when criteria.Pornstars.Count == 1 => GeneratePornstarUrl(criteria.Pornstars),
            { Categories: [], Tags: [], Pornstars: [] } => new PageLink { Url = "/videos/all" },
            { Paging.SearchOrder: SearchOrder.Clicks } => new PageLink { Url = "/videos/best" },
            _ => new PageLink { Url = "/videos/all" }
        };

        if (pageLink == null) return null;

        var page = criteria.Paging.PageIndex == 1 
            ? "" 
            : criteria.Terms != null 
                ? $"&page={criteria.Paging.PageIndex}" 
                : $"/{criteria.Paging.PageIndex}";

        return pageLink with { Url = $"{pageLink.Url}{page}" };
    }

    private PageLink GeneratePornstarUrl(List<Pornstar> pornstars)
        => new()
        {
            Url = $"/videos/pornstars/{Slugify(pornstars.First().Value!)}"
        };

    private PageLink GenerateCategoryUrl(List<Category> categories)
        => new()
        {
            Url = $"/videos/categories/{Slugify(categories.First().Value!)}"
        };

    private PageLink GenerateTagUrl(List<Tag> tags)
        => new()
        {
            Url = $"/videos/tags/{Slugify(tags.First().Value!)}"
        };

    private string Slugify(string value)
    {
        if (value == null) return "error";
        var splitted = value.Split(' ').Select(split => split.ToLower());
        return string.Join("-", splitted);
    }
}