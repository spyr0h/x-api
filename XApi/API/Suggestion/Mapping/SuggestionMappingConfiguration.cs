using Mapster;
using XApi.API.Suggestion.DTO;
using XApi.API.Videos.DTO;
using XApi.Core.Suggestion.Models;

namespace XApi.API.Suggestion.Mapping;

public class SuggestionMappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<SuggestionBox, SuggestionBoxDTO>()
            .MapWith(suggestionBox => new SuggestionBoxDTO
            {
                Title = suggestionBox.Title,
                Order = suggestionBox.Order,
                Category = suggestionBox.Category,
                SuggestedVideos = suggestionBox.SuggestedVideos.Select(video => video.Adapt<VideoDTO>()).ToArray()
            });
    }
}
