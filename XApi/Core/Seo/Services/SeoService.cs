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
    public SeoData ProvideSeoData(SearchCriteria searchCriteria)
    {
        return new()
        {
            Title = titleBuilder.BuildFrom(searchCriteria),
            Description = descriptionBuilder.BuildFrom(searchCriteria),
            Headline = headLineBuilder.BuildFrom(searchCriteria),
            Canonical = pageLinkProvider.ProvidePageLink(searchCriteria).Url,
            IsIndexed = false
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
}
