namespace XApi.API.Search.DTO;

public record SearchPagingSpecsDTO
{
    public int PageIndex { get; set; }
    public int ResultsPerPage { get; set; } = 25;
}
