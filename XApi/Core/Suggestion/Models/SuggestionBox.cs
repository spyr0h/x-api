using XApi.Core.Suggestion.Enums;
using XApi.Core.Videos.Models;

namespace XApi.Core.Suggestion.Models;

public record SuggestionBox
{
    public string? Title { get; set; }
    public int Order { get; set; }
    public SuggestionCategory Category { get; set; }
    public Video[] SuggestedVideos { get; set; } = [];
}
