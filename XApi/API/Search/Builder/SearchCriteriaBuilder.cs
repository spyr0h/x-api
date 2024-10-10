using Mapster;
using XApi.API.Search.Builder.Interfaces;
using XApi.API.Search.DTO;
using XApi.Core.Pornstars.Ports.Interfaces;
using XApi.Core.Search.Models;
using XApi.Core.Tags.Ports.Interfaces;

namespace XApi.API.Search.Builder;

public class SearchCriteriaBuilder(ITagService tagService, IPornstarService pornstarService) : ISearchCriteriaBuilder
{
    public async Task<SearchCriteria> BuildFrom(SearchCriteriaDTO searchCriteriaDTO)
    {
        var tags = searchCriteriaDTO.TagsIDS.Count != 0
            ? await tagService.ProvideTagsForIds([.. searchCriteriaDTO.TagsIDS])
            : [];

        var pornstars = searchCriteriaDTO.PornstarsIDS.Count != 0
            ? await pornstarService.ProvidePornstarsForIds([.. searchCriteriaDTO.PornstarsIDS])
            : [];

        return new()
        {
            Tags = [.. tags],
            Pornstars = [.. pornstars],
            Paging = searchCriteriaDTO.Paging.Adapt<SearchPagingSpecs>()
        };
    }
}
