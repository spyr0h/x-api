using XApi.Core.Search.Enums;

namespace XApi.API.Search.DTO;

public record SearchPagingSpecsDTO
{
    public int PageIndex { get; set; }
    public SearchOrder SearchOrder { get; set; } = SearchOrder.Date;
    public int ResultsPerPage { get; set; } = 25;
}
