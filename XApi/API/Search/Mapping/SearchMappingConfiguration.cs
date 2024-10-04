using Mapster;
using XApi.API.Search.DTO;
using XApi.API.Videos.DTO;
using XApi.Core.Search.Models;

namespace XApi.API.Search.Mapping;

public class SearchMappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<SearchCriteriaDTO, SearchCriteria>()
            .MapWith(searchCriteria => new SearchCriteria
            {
                TagsIDS = searchCriteria.TagsIDS,
                PornstarsIDS = searchCriteria.PornstarsIDS,
                Paging = searchCriteria.Paging.Adapt<SearchPaging>()
            });

        config.NewConfig<SearchResult, SearchResultDTO>()
            .MapWith(searchResult => new SearchResultDTO
            {
                Count = searchResult.Count,
                Videos = searchResult.Videos.Select(video => video.Adapt<VideoDTO>()).ToList()
            });

        config.NewConfig<SearchPagingDTO, SearchPaging>()
            .Map(dest => dest.PageIndex, src => src.PageIndex)
            .Map(dest => dest.ResultsPerPage, src => src.ResultsPerPage);
    }
}
