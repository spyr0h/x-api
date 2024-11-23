using Mapster;
using XApi.API.Seo.DTO;
using XApi.Core.Seo.Models;

namespace XApi.API.Seo.Mapping;

public class SeoMappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<SeoData, SeoDataDTO>()
            .Map(dest => dest.Title, src => src.Title)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.Headline, src => src.Headline)
            .Map(dest => dest.Canonical, src => src.Canonical)
            .Map(dest => dest.IsIndexed, src => src.IsIndexed)
            .Map(dest => dest.RecentCount, src => src.RecentCount);
    }
}
