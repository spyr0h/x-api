using XApi.API.Videos.DTO;

namespace XApi.API.Search.DTO;

public record SearchResultDTO
{
    public int GlobalCount { get; set; }    
    public int Count { get; set; }
    public List<VideoDTO> Videos { get; set; } = [];
}
