using Mapster;
using XApi.API.Page.DTO;
using XApi.Core.Page.Models;

namespace XApi.API.Page.Mapping;

public class PageMappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<PageLinkDTO, PageLink>()
            .Map(dest => dest.Url, src => src.Url);
    }
}
