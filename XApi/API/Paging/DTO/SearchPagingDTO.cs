namespace XApi.API.Paging.DTO;

public record SearchPagingDTO
{
    public List<SearchPageDTO> Pages { get; set; } = [];
    public SearchPageDTO? PreviousPage { get; set; }
    public SearchPageDTO? NextPage { get; set; }  
    public SearchPageDTO? FirstPage { get; set; }
    public SearchPageDTO? LastPage { get; set; }  
}
