using XApi.Core.Search.Models;

namespace XApi.Core.Search.Ports.Interfaces;

public interface ISearchService
{
    public Task<SearchResult> SearchVideosByCriteria(SearchCriteria searchCriteria);
}
