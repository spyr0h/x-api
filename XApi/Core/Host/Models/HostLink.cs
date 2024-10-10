using XApi.Core.Host.Enums;

namespace XApi.Core.Host.Models;

public record HostLink
{
    public string? Url { get; set; }
    public double? Size { get; set; }
    public Enums.Host? Host { get; set; }
    public Resolution? Resolution { get; set; }
    public Format? Format { get; set; }
    public int? Part { get; set; }
}
