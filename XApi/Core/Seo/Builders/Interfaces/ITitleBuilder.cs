using XApi.Core.Search.Models;

namespace XApi.Core.Seo.Builders.Interfaces;

public interface ITitleBuilder
{
    public string BuildFrom(SearchCriteria criteria, SearchResult searchResult);
}
