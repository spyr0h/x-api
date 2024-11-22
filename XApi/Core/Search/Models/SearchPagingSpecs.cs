using XApi.Core.Search.Enums;

namespace XApi.Core.Search.Models;

public record SearchPagingSpecs
{
    public int PageIndex { get; set; }
    public SearchOrder SearchOrder { get; set; } = SearchOrder.Date;
    public int ResultsPerPage { get; set; } = 25;
}
