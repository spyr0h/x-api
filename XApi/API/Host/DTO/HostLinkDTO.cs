using XApi.Core.Host.Enums;
using LinkHost = XApi.Core.Host.Enums.Host;

namespace XApi.API.Host.DTO;

public class HostLinkDTO
{
    public string? Url { get; set; }
    public double? Size { get; set; }
    public LinkHost? Host { get; set; }
    public Resolution? Resolution { get; set; }
    public Format? Format { get; set; }
    public int? Part { get; set; }
}
