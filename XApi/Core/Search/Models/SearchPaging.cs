namespace XApi.Core.Search.Models;

public record SearchPaging
{
    public int PageIndex { get; set; }
    public int ResultsPerPage { get; set; } = 25;
}
