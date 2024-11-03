using XApi.Core.Videos.Models;

namespace XApi.Core.Suggestion.Ports.Interfaces;

public interface ISuggestionProvider
{
    Task<Video[]> ProvideSuggestedVideos(Video video);
}