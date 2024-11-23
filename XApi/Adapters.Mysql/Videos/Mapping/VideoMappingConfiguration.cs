using Mapster;
using XApi.Adapters.Mysql.Videos.Models;
using XApi.Core.Pictures.Models;
using XApi.Core.Host.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace XApi.Adapters.Mysql.Videos.Mapping;

public class VideoMappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Video, Core.Videos.Models.Video>()
            .MapWith(video => ConvertToCoreVideo(video));
    }

    private Core.Videos.Models.Video ConvertToCoreVideo(Video video)
        => new Core.Videos.Models.Video
        {
            ID = video.VideoID,
            Title = video.Title,
            Description = video.Description,
            Duration = video.Duration,
            Year = video.Year,
            New = video.ModifiedDate != null 
                && ((DateTime.Now - video.ModifiedDate!.Value).TotalDays < 1),
            Tags = video.Tags?
                    .Split('|')
                    .Select(tag =>
                    {
                        var splittedTag = tag.Split('µ');
                        return new Core.Tags.Models.Tag
                        {
                            ID = GetIntValue(splittedTag[0])!.Value,
                            Value = GetStrValue(splittedTag[1])
                        };
                    })
                    .ToList() ?? [],
            Categories = video.Categories?
                    .Split('|')
                    .Select(category =>
                    {
                        var splittedCategory = category.Split('µ');
                        return new Core.Categories.Models.Category
                        {
                            ID = GetIntValue(splittedCategory[0])!.Value,
                            Value = GetStrValue(splittedCategory[1])
                        };
                    })
                    .ToList() ?? [],
            Pornstars = video.Pornstars?
                    .Split('|')
                    .Select(pornstar =>
                    {
                        var splittedPornstar = pornstar.Split('µ');
                        return new Core.Pornstars.Models.Pornstar
                        {
                            ID = GetIntValue(splittedPornstar[0])!.Value,
                            Value = GetStrValue(splittedPornstar[1])
                        };
                    })
                    .ToList() ?? [],
            Links = video.Links?
                    .Split('|')
                    .Select(link =>
                    {
                        var splittedLink = link.Split('µ');
                        return new Core.Host.Models.HostLink
                        {
                            Url = GetStrValue(splittedLink[0]),
                            Size = GetDoubleValue(splittedLink[1]),
                            Host = GetValue<Host>(splittedLink[2]),
                            Resolution = GetValue<Resolution>(splittedLink[3]),
                            Format = GetValue<Format>(splittedLink[4]),
                            Part = GetIntValue(splittedLink[5])
                        };
                    })
                    .ToList() ?? [],
            Pictures = video.Pictures?
                    .Split('|')
                    .Select(picture =>
                    {
                        var splittedPicture = picture.Split('µ');
                        return new Picture
                        {
                            DirectUrl = GetStrValue(splittedPicture[0]),
                            HostUrl = GetStrValue(splittedPicture[1])
                        };
                    })
                    .ToList() ?? [],
        };

    private string GetStrValue(string value)
    {
        if (value == "NULL") return null;
        return value;
    }

    private int? GetIntValue(string value)
    {
        if (value == "NULL") return null;
        return int.TryParse(value, out int part) ? part : null;
    }

    private double? GetDoubleValue(string value)
    {
        if (value == "NULL") return null;
        return double.TryParse(value, out double part) ? part : null;
    }

    private T? GetValue<T>(string enumValue) where T : Enum
    {
        if (enumValue == "NULL") return default;
        var parsed = int.TryParse(enumValue, out var value);
        if (!parsed || !Enum.IsDefined(typeof(T), value)) return default;

        return (T)(object)value;
    }
}