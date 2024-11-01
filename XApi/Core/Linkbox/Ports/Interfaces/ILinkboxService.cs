using XApi.Core.Search.Models;
using XApi.Core.Videos.Models;

namespace XApi.Core.Linkbox.Ports.Interfaces;

public interface ILinkboxService
{
    public Task<Models.Linkbox[]> ProvideLinkboxes(SearchCriteria criteria);
    public Task<Models.Linkbox[]> ProvideLinkboxes(Video video);
}