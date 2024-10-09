using XApi.API.Search.DTO;
using XApi.Core.Search.Models;

namespace XApi.API.Search.Builder.Interfaces;

public interface ISearchCriteriaBuilder
{
    public Task<SearchCriteria> BuildFrom(SearchCriteriaDTO searchCriteriaDTO);
}
