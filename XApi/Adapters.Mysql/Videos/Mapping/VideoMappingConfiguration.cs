using Mapster;
using XApi.Adapters.Mysql.Videos.Models;
using XApi.Core.Links.Enums;
using XApi.Core.Links.Models;
using XApi.Core.Pornstars.Models;
using XApi.Core.Tags.Models;
using XApi.Common.Extensions;
using XApi.Core.Pictures.Models;

namespace XApi.Adapters.Mysql.Videos.Mapping;

public class VideoMappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Video, Core.Videos.Models.Video>()
            .MapWith(video => new Core.Videos.Models.Video
            {
                ID = video.VideoID,
                Title = video.Title,
                Description = video.Description,
                Duration = video.Duration,
                Year = video.Year,
                Tags = video.Tags.Select(tag => tag.Adapt<Tag>()).ToList(),
                Pornstars = video.Pornstars.Select(pornstar => pornstar.Adapt<Pornstar>()).ToList(),
                Links = video.Links.Select(link => link.Adapt<HostLink>()).ToList(),
                Pictures = video.Pictures.Select(picture => picture.Adapt<Picture>()).ToList(),
            });
    }
}