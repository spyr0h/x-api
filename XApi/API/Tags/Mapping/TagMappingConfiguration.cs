﻿using Mapster;
using XApi.API.Tags.DTO;
using XApi.Core.Tags.Models;

namespace XApi.API.Tags.Mapping;

public class TagMappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Tag, TagDTO>()
            .Map(dest => dest.ID, src => src.ID)
            .Map(dest => dest.Value, src => src.Value)
            .Map(dest => dest.Count, src => src.Count)
            .Map(dest => dest.RecentCount, src => src.RecentCount);

        config.NewConfig<TagAutocompleteDTO, TagAutocomplete>()
            .Map(dest => dest.Value, src => src.Value);
    }
}
