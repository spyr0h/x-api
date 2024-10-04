using XApi.Core.Search.Models;
using XApi.Core.Search.Ports.Interfaces;

namespace XApi.Core.Search.Services;

public class SearchService(ISearchProvider searchProvider) : ISearchService
{
    public async Task<SearchResult> SearchVideosByCriteria(SearchCriteria searchCriteria)
    {
        var result = await searchProvider.SearchVideosByCriteria(searchCriteria);
        return result with { Count = result.Videos.Count() };
    }
}
