using XApi.Core.Search.Models;
using XApi.Core.Seo.Builders.Interfaces;
using XApi.Core.Seo.Models;
using XApi.Core.Seo.Ports.Interfaces;

namespace XApi.Core.Seo.Services;

public class SeoService(
    ITitleBuilder titleBuilder, 
    IDescriptionBuilder descriptionBuilder, 
    IHeadLineBuilder headLineBuilder
    ) : ISeoService
{
    public SeoData ProvideSeoData(SearchCriteria searchCriteria)
    {
        return new()
        {
            Title = titleBuilder.BuildFrom(searchCriteria),
            Description = descriptionBuilder.BuildFrom(searchCriteria),
            Headline = headLineBuilder.BuildFrom(searchCriteria),
            Canonical = "",
            IsIndexed = false
        };
    }
}
