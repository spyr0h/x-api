using XApi.Core.Linkbox.Enums;

namespace XApi.Core.Linkbox.Models;

public record Linkbox
{
    public string? Title { get; set; }
    public LinkboxCategory Category { get; set; }
    public int Order { get; set; }
    public LinkboxLink[] Links { get; set; } = [];
}
