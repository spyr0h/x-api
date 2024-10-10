using XApi.Core.Search.Models;

namespace XApi.Core.Paging.Ports.Interfaces;

public interface IPagingService
{
    public Task<SearchPagingSpecs> CalculatePagingFromSearchData(SearchCriteria searchCriteria, SearchResult searchResult);
}
