using XApi.API.Categories.DTO;
using XApi.API.Host.DTO;
using XApi.API.Pictures.DTO;
using XApi.API.Pornstars.DTO;
using XApi.API.Tags.DTO;

namespace XApi.API.Videos.DTO;

public record VideoDTO
{
    public int ID { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public TimeSpan? Duration { get; set; }
    public int? Year { get; set; }
    public List<TagDTO> Tags { get; set; } = [];
    public List<CategoryDTO> Categories { get; set; } = [];
    public List<PornstarDTO> Pornstars { get; set; } = [];
    public List<HostLinkDTO> Links { get; set; } = [];
    public List<PictureDTO> Pictures { get; set; } = [];
}
