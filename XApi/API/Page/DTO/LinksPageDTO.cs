using XApi.API.Linkbox.DTO;
using XApi.API.Seo.DTO;

namespace XApi.API.Page.DTO;

public class LinksPageDTO
{
    public SeoDataDTO? SeoData { get; set; }
    public LinkboxesDTO? Linkboxes { get; set; }
    public PageLinkExtendedDTO[] PageLinks { get; set; } = [];
}
