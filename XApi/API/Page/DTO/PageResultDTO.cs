using XApi.API.Linkbox.DTO;
using XApi.API.Paging.DTO;
using XApi.API.Search.DTO;
using XApi.API.Seo.DTO;

namespace XApi.API.Page.DTO;

public record PageResultDTO
{
    public SearchResultDTO? SearchResult { get; set; }
    public SeoDataDTO? SeoData { get; set; }
    public SearchPagingDTO? SearchPaging { get; set; }   
    public LinkboxesDTO? Linkboxes { get; set; }
}
