namespace XApi.API.Search.DTO;

public record SearchCriteriaDTO
{
    public List<int> CategoriesIDS { get; set; } = [];
    public List<int> TagsIDS { get; set; } = [];
    public List<int> PornstarsIDS { get; set; } = [];
    public SearchPagingSpecsDTO Paging { get; set; } = new() { PageIndex = 1 };
}
