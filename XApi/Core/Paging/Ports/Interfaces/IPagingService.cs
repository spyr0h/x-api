using XApi.Core.Paging.Models;
using XApi.Core.Search.Models;

namespace XApi.Core.Paging.Ports.Interfaces;

public interface IPagingService
{
    public Task<SearchPaging> CalculatePagingFromSearchData(SearchCriteria searchCriteria, SearchResult searchResult);
}
