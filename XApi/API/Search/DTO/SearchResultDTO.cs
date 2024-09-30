using XApi.API.Videos.DTO;

namespace XApi.API.Search.DTO;

public record SearchResultDTO
{
    public List<VideoDTO> Videos { get; set; } = [];
}
