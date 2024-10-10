using Mapster;
using XApi.API.Search.DTO;
using XApi.API.Videos.DTO;
using XApi.Core.Search.Models;

namespace XApi.API.Search.Mapping;

public class SearchMappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<SearchResult, SearchResultDTO>()
            .MapWith(searchResult => new SearchResultDTO
            {
                Count = searchResult.Count,
                Videos = searchResult.Videos.Select(video => video.Adapt<VideoDTO>()).ToList()
            });

        config.NewConfig<SearchPagingSpecsDTO, SearchPagingSpecs>()
            .Map(dest => dest.PageIndex, src => src.PageIndex)
            .Map(dest => dest.ResultsPerPage, src => src.ResultsPerPage);
    }
}
