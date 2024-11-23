namespace XApi.API.Seo.DTO;

public record SeoDataDTO
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Headline { get; set; }
    public string? Canonical { get; set; }
    public bool IsIndexed { get; set; }
    public int RecentCount { get; set; }
}
