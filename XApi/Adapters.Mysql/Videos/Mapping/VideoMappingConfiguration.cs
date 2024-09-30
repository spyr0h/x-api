using Mapster;
using XApi.Adapters.Mysql.Videos.Models;
using XApi.Core.Pornstars.Models;
using XApi.Core.Tags.Models;

namespace XApi.Adapters.Mysql.Videos.Mapping;

public class VideoMappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<VideoTag, Tag>()
            .Map(dest => dest.ID, src => src.TagID)
            .Map(dest => dest.Value, src => src.TagValue);

        config.NewConfig<VideoPornstar, Pornstar>()
            .Map(dest => dest.ID, src => src.PornstarID)
            .Map(dest => dest.Value, src => src.PornstarValue);

        config.NewConfig<Video, Core.Videos.Models.Video>()
            .MapWith(video => new Core.Videos.Models.Video
            {
                ID = video.VideoID,
                Title = video.Title,
                Description = video.Description,
                Duration = video.Duration,
                Year = video.Year,
                Tags = video.Tags.Select(tag => tag.Adapt<Tag>()).ToList(),
                Pornstars = video.Pornstars.Select(pornstar => pornstar.Adapt<Pornstar>()).ToList()
            });
    }
}