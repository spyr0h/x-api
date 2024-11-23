using Mapster;
using XApi.API.Linkbox.DTO;
using XApi.Core.Linkbox.Models;

namespace XApi.API.Linkbox.Mapping;

public class LinkboxMappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Core.Linkbox.Models.Linkbox[], LinkboxesDTO>()
            .MapWith(linkboxes => new LinkboxesDTO
            {
                Linkboxes = linkboxes.Select(linkbox => linkbox.Adapt<LinkboxDTO>()).ToArray()
            });

        config.NewConfig<LinkboxLink, LinkboxLinkDTO>()
            .Map(dest => dest.Url, src => src.Url)
            .Map(dest => dest.LinkText, src => src.LinkText)
            .Map(dest => dest.Order, src => src.Order)
            .Map(dest => dest.Count, src => src.Count)
            .Map(dest => dest.RecentCount, src => src.RecentCount);

        config.NewConfig<Core.Linkbox.Models.Linkbox, LinkboxDTO>()
            .MapWith(linkbox => new LinkboxDTO
            {
                Title = linkbox.Title,
                Order = linkbox.Order,
                Category = (int)linkbox.Category,
                Links = linkbox.Links.Select(link => link.Adapt<LinkboxLinkDTO>()).ToArray()  
            });
    }
}
