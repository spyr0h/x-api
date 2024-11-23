namespace XApi.API.Pornstars.DTO;

public record PornstarDTO
{
    public int ID { get; set; }
    public string? Value { get; set; }
    public int Count { get; set; }
    public int RecentCount { get; set; }
}
