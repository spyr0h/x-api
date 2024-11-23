namespace XApi.Core.Linkbox.Models;

public record LinkboxLink
{
    public string? Url { get; set; }
    public string? LinkText { get; set; }
    public int Order { get; set; }
    public int? Count { get; set; }
    public int? RecentCount { get; set; }
}
