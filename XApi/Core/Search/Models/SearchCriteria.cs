using XApi.Core.Pornstars.Models;
using XApi.Core.Tags.Models;

namespace XApi.Core.Search.Models;

public record SearchCriteria
{
    public List<Tag> Tags { get; set; } = [];
    public List<Pornstar> Pornstars { get; set; } = [];
    public SearchPaging Paging { get; set; } = new() { PageIndex = 1 };
}
