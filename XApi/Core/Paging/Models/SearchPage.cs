using XApi.Core.Page.Models;

namespace XApi.Core.Paging.Models;

public class SearchPage
{
    public int Number { get; set; }
    public PageLink? Url { get; set; }
}
