using XApi.Core.Search.Models;
using XApi.Core.Seo.Models;

namespace XApi.Core.Seo.Ports.Interfaces;

public interface ISeoService
{
    SeoData ProvideSeoData(SearchCriteria searchCriteria);
}
