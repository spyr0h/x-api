using XApi.Core.Page.Models;
using XApi.Core.Search.Models;
using XApi.Core.Videos.Models;

namespace XApi.Core.Page.Ports.Interfaces;

public interface IPageLinkProvider
{
    public PageLink? ProvidePageLink(SearchCriteria criteria);
    public PageLink? ProvidePageLink(Video video);
}