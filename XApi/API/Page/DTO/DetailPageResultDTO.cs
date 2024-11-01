using XApi.API.Linkbox.DTO;
using XApi.API.Seo.DTO;
using XApi.Core.Videos.Models;

namespace XApi.API.Page.DTO;

public record DetailPageResultDTO
{
    public Video? Video { get; set; }
    public SeoDataDTO? SeoData { get; set; }
    public LinkboxesDTO? Linkboxes { get; set; }
}
