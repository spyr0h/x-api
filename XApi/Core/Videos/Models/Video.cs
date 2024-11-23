using XApi.Core.Categories.Models;
using XApi.Core.Host.Models;
using XApi.Core.Pictures.Models;
using XApi.Core.Pornstars.Models;
using XApi.Core.Tags.Models;

namespace XApi.Core.Videos.Models;

public record Video
{
    public int ID { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public TimeSpan? Duration { get; set; }
    public int? Year { get; set; }
    public bool New { get; set; }
    public List<Tag> Tags { get; set; } = [];
    public List<Category> Categories { get; set; } = [];
    public List<Pornstar> Pornstars { get; set; } = [];
    public List<HostLink> Links { get; set; } = [];
    public List<Picture> Pictures { get; set; } = [];
}
