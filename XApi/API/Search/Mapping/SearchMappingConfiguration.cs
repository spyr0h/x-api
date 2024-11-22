using Mapster;
using XApi.API.Search.DTO;
using XApi.API.Videos.DTO;
using XApi.Core.Search.Models;

namespace XApi.API.Search.Mapping;

public class SearchMappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<SearchCriteria, SearchCriteriaDTO>()
            .MapWith(searchCriteria => new SearchCriteriaDTO
            {
                CategoriesIDS = searchCriteria.Categories.Select(category => category.ID).ToList(),
                TagsIDS = searchCriteria.Tags.Select(tag => tag.ID).ToList(),
                PornstarsIDS = searchCriteria.Pornstars.Select(pornstar => pornstar.ID).ToList(),
                Paging = searchCriteria.Paging.Adapt<SearchPagingSpecsDTO>()
            });

        config.NewConfig<SearchPagingSpecs, SearchPagingSpecsDTO>()
            .MapWith(searchPagingSpecs => new SearchPagingSpecsDTO
            {
                PageIndex = searchPagingSpecs.PageIndex,
                SearchOrder = searchPagingSpecs.SearchOrder,
                ResultsPerPage = searchPagingSpecs.ResultsPerPage
            });

        config.NewConfig<SearchResult, SearchResultDTO>()
            .MapWith(searchResult => new SearchResultDTO
            {
                GlobalCount = searchResult.GlobalCount,
                Count = searchResult.Count,
                Videos = searchResult.Videos.Select(video => video.Adapt<VideoDTO>()).ToList()
            });

        config.NewConfig<SearchPagingSpecsDTO, SearchPagingSpecs>()
            .Map(dest => dest.PageIndex, src => src.PageIndex)
            .Map(dest => dest.SearchOrder, src => src.SearchOrder)
            .Map(dest => dest.ResultsPerPage, src => src.ResultsPerPage);
    }
}
