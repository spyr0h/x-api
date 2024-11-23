using Mapster;
using XApi.API.Pornstars.DTO;
using XApi.Core.Pornstars.Models;

namespace XApi.API.Pornstars.Mapping;

public class PornstarMappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Pornstar, PornstarDTO>()
            .Map(dest => dest.ID, src => src.ID)
            .Map(dest => dest.Value, src => src.Value)
            .Map(dest => dest.Count, src => src.Count)
            .Map(dest => dest.RecentCount, src => src.RecentCount);

        config.NewConfig<PornstarAutocompleteDTO, PornstarAutocomplete>()
            .Map(dest => dest.Value, src => src.Value);
    }
}
