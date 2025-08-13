using XApi.Core.Page.Enums;
using XApi.Core.Search.Models;
using XApi.Core.Seo.Models;
using XApi.Core.Videos.Models;

namespace XApi.Core.Seo.Ports.Interfaces;

public interface ISeoService
{
    SeoData ProvideSeoData(SearchCriteria searchCriteria, SearchResult searchResult);
    SeoData ProvideSeoData(Video video);
    SeoData ProvideSeoData(ListPageType listPageType);
}
