using XApi.Core.Autocomplete.Interfaces;

namespace XApi.Core.Pornstars.Models;

public record Pornstar : IAutocompletable
{
    public int ID { get; set; }
    public string? Value { get; set; }
    public int Count { get; set; }
    public int RecentCount { get; set; }
}
