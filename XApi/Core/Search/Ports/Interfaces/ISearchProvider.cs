using XApi.Core.Search.Models;

namespace XApi.Core.Search.Ports.Interfaces;

public interface ISearchProvider
{
    public Task<SearchResult> SearchVideosByCriteria(SearchCriteria searchCriteria);
}
