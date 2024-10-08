﻿using Mapster;
using XApi.API.Host.DTO;
using XApi.Core.Host.Models;

namespace XApi.API.Host.Mapping;

public class LinkMappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<HostLink, HostLinkDTO>()
            .Map(dest => dest.Url, src => src.Url)
            .Map(dest => dest.Size, src => src.Size)
            .Map(dest => dest.Host, src => src.Host)
            .Map(dest => dest.Resolution, src => src.Resolution)
            .Map(dest => dest.Format, src => src.Format)
            .Map(dest => dest.Part, src => src.Part);
    }
}
