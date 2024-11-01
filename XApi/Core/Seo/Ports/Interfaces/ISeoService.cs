using XApi.Core.Search.Models;
using XApi.Core.Seo.Models;
using XApi.Core.Videos.Models;

namespace XApi.Core.Seo.Ports.Interfaces;

public interface ISeoService
{
    SeoData ProvideSeoData(SearchCriteria searchCriteria);
    SeoData ProvideSeoData(Video video);
}
