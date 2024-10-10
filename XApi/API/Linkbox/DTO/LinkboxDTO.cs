namespace XApi.API.Linkbox.DTO;

public record LinkboxDTO
{
    public string? Title { get; set; }
    public int Category { get; set; }
    public int Order { get; set; }
    public LinkboxLinkDTO[]? Links { get; set; }
}
