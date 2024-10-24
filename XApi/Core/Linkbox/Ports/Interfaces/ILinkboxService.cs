using XApi.Core.Search.Models;

namespace XApi.Core.Linkbox.Ports.Interfaces;

public interface ILinkboxService
{
    public Task<Models.Linkbox[]> ProvideLinkboxes(SearchCriteria criteria);
}