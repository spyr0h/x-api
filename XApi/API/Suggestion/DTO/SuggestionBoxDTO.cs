using XApi.API.Videos.DTO;
using XApi.Core.Suggestion.Enums;

namespace XApi.API.Suggestion.DTO;

public record SuggestionBoxDTO
{
    public string? Title { get; set; }
    public int Order { get; set; }
    public SuggestionCategory Category { get; set; }
    public VideoDTO[]? SuggestedVideos { get; set; }
}
