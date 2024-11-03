using XApi.Core.Suggestion.Models;
using XApi.Core.Videos.Models;

namespace XApi.Core.Suggestion.Ports.Interfaces;

public interface ISuggestionService
{
    Task<SuggestionBox[]> ProvideSuggestions(Video video);
}