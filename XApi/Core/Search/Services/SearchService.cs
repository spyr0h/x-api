using XApi.Core.Search.Models;
using XApi.Core.Search.Ports.Interfaces;

namespace XApi.Core.Search.Services;

public class SearchService(ISearchProvider searchProvider) : ISearchService
{
    public Task<SearchResult> SearchVideosByCriteria(SearchCriteria searchCriteria)
        => searchProvider.SearchVideosByCriteria(searchCriteria);
}
