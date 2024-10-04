namespace XApi.API.Search.DTO;

public record SearchCriteriaDTO
{
    public List<int> TagsIDS { get; set; } = [];
    public List<int> PornstarsIDS { get; set; } = [];
    public SearchPagingDTO Paging { get; set; } = new() { PageIndex = 1 };
}
