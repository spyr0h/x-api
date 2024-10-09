using XApi.API.Search.DTO;
using XApi.API.Seo.DTO;

namespace XApi.API.Page.DTO;

public record PageResultDTO
{
    public SearchResultDTO? SearchResult { get; set; }
    public SeoDataDTO? SeoData { get; set; }
}
