using XApi.Core.Links.Enums;
using Host = XApi.Core.Links.Enums.Host;

namespace XApi.API.Links.DTO;

public class LinkDTO
{
    public string? Url { get; set; }
    public double? Size { get; set; }
    public Host? Host { get; set; }
    public Resolution? Resolution { get; set; }
    public Format? Format { get; set; }
    public int? Part { get; set; }
}
