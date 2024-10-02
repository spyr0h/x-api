using Mapster;
using XApi.API.Links.DTO;
using XApi.API.Pictures.DTO;
using XApi.API.Pornstars.DTO;
using XApi.API.Tags.DTO;
using XApi.API.Videos.DTO;
using XApi.Core.Videos.Models;

namespace XApi.API.Videos.Mapping;

public class VideoMappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Video, VideoDTO>()
            .MapWith(video => new VideoDTO
            {
                ID = video.ID,
                Title = video.Title,
                Description = video.Description,
                Duration = video.Duration,
                Year = video.Year,
                Tags = video.Tags.Select(tag => tag.Adapt<TagDTO>()).ToList(),
                Pornstars = video.Pornstars.Select(pornstar => pornstar.Adapt<PornstarDTO>()).ToList(),
                Links = video.Links.Select(link => link.Adapt<LinkDTO>()).ToList(),
                Pictures = video.Pictures.Select(picture => picture.Adapt<PictureDTO>()).ToList()
            });
    }
}
