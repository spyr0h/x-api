using XApi.Core.Page.Enums;
using XApi.Core.Page.Ports.Interfaces;
using XApi.Core.Search.Models;
using XApi.Core.Seo.Builders.Interfaces;
using XApi.Core.Seo.Models;
using XApi.Core.Seo.Ports.Interfaces;
using XApi.Core.Videos.Models;

namespace XApi.Core.Seo.Services;

public class SeoService(
    ITitleBuilder titleBuilder, 
    IDescriptionBuilder descriptionBuilder, 
    IHeadLineBuilder headLineBuilder,
    IPageLinkProvider pageLinkProvider
    ) : ISeoService
{
    public SeoData ProvideSeoData(SearchCriteria searchCriteria, SearchResult searchResult)
    {
        return new()
        {
            Title = titleBuilder.BuildFrom(searchCriteria, searchResult),
            Description = descriptionBuilder.BuildFrom(searchCriteria, searchResult),
            Headline = headLineBuilder.BuildFrom(searchCriteria, searchResult),
            Canonical = pageLinkProvider.ProvidePageLink(searchCriteria).Url,
            IsIndexed = true,
            RecentCount = searchResult.RecentCount
        };
    }

    public SeoData ProvideSeoData(Video video)
    {
        return new()
        {
            Title = video.Title,
            Description = video.Title,
            Headline = video.Title,
            Canonical = "canonical-url",
            IsIndexed = false
        };
    }

    public SeoData ProvideSeoData(ListPageType listPageType)
    {
        return new()
        {
            Title = listPageType == ListPageType.Category ? "All categories" : "All actresses and actors",
            Description = listPageType == ListPageType.Category ? "All categories" : "All actresses and actors",
            Headline = listPageType == ListPageType.Category ? "All categories" : "All actresses and actors",
            Canonical = "canonical-url",
            IsIndexed = true
        };
    }
}
