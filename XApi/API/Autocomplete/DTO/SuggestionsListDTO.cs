namespace XApi.API.Autocomplete.DTO;

public record SuggestionsListDTO
{
    public SuggestionDTO[] Suggestions { get; set; } = [];
}
