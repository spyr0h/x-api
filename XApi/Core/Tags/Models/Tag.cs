using XApi.Core.Autocomplete.Interfaces;

namespace XApi.Core.Tags.Models;

public record Tag : IAutocompletable
{
    public int ID { get; set; }
    public string? Value { get; set; }
    public int Count { get; set; }
}
