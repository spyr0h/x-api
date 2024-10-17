using Mapster;
using XApi.API.Autocomplete.DTO;
using XApi.API.Tags.DTO;
using XApi.Core.Pornstars.Models;
using XApi.Core.Tags.Models;

namespace XApi.API.Autocomplete.Mapping;

public class AutocompleteMappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<FullAutocompleteDTO, TagAutocomplete>()
            .Map(dest => dest.Value, src => src.Value);

        config.NewConfig<FullAutocompleteDTO, PornstarAutocomplete>()
            .Map(dest => dest.Value, src => src.Value);
    }
}
