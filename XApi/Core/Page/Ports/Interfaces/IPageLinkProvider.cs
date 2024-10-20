using XApi.Core.Page.Models;
using XApi.Core.Search.Models;

namespace XApi.Core.Page.Ports.Interfaces;

public interface IPageLinkProvider
{
    public PageLink? ProvidePageLink(SearchCriteria criteria);
}