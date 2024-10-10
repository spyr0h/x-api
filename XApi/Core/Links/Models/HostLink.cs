using XApi.Core.Links.Enums;

namespace XApi.Core.Links.Models;

public record HostLink
{
    public string? Url { get; set; }
    public double? Size { get; set; }
    public Host? Host { get; set; }
    public Resolution? Resolution { get; set; }
    public Format? Format { get; set; }
    public int? Part { get; set; }
}
