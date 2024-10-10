namespace XApi.Core.Search.Models;

public record SearchPagingSpecs
{
    public int PageIndex { get; set; }
    public int ResultsPerPage { get; set; } = 25;
}
