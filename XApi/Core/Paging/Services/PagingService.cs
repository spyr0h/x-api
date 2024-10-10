using XApi.Core.Paging.Ports.Interfaces;
using XApi.Core.Search.Models;

namespace XApi.Core.Paging.Services;

public class PagingService : IPagingService
{
    public Task<SearchPagingSpecs> CalculatePagingFromSearchData(SearchCriteria searchCriteria, SearchResult searchResult)
    {
        throw new NotImplementedException();
    }
}
