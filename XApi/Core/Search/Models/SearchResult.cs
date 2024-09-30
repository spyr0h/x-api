using XApi.Core.Videos.Models;

namespace XApi.Core.Search.Models;

public record SearchResult
{
    public List<Video> Videos { get; set; } = []; 
}
