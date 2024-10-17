using XApi.Core.Autocomplete.Enums;

namespace XApi.API.Autocomplete.DTO;

public record SuggestionDTO
{
    public string? Value { get; set; }
    public SuggestionType Type { get; set; }
    public string SearchUrl { get; set; }
}
