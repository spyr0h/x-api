using XApi.Core.Autocomplete.Interfaces;

namespace XApi.Core.Categories.Models;

public record Category : IAutocompletable
{
    public int ID { get; set; }
    public string? Value { get; set; }
    public int Count { get; set; }
    public int RecentCount { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
}
