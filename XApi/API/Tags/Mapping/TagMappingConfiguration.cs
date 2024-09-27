using Mapster;
using XApi.API.Tags.DTO;
using XApi.Core.Tags.Models;

namespace API.Tags.Mapping;

public class TagMappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Tag, TagDTO>()
            .Map(dest => dest.ID, src => src.ID)
            .Map(dest => dest.Value, src => src.Value);

        config.NewConfig<TagAutocompleteDTO, TagAutocomplete>()
            .Map(dest => dest.Value, src => src.Value);
    }
}
