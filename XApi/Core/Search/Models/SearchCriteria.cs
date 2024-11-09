using XApi.Core.Categories.Models;
using XApi.Core.Pornstars.Models;
using XApi.Core.Tags.Models;

namespace XApi.Core.Search.Models;

public record SearchCriteria
{
    public string? Terms { get; set; }
    public List<Category> Categories { get; set; } = [];
    public List<Tag> Tags { get; set; } = [];
    public List<Pornstar> Pornstars { get; set; } = [];
    public SearchPagingSpecs Paging { get; set; } = new() { PageIndex = 1, ResultsPerPage = 25 };
}
