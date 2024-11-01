using XApi.Core.Page.Models;
using XApi.Core.Search.Models;

namespace XApi.Core.Page.Ports.Interfaces;

public interface IPageRoutingService
{
    public Task<SearchCriteria?> RoutePageLinkToCriteria(PageLink pageLink);
    public Task<int?> RoutePageLinkToVideoId(PageLink pageLink);
}