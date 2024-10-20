using Mapster;
using XApi.API.Paging.DTO;
using XApi.Core.Paging.Models;

namespace XApi.API.Paging.Mapping;

public class PagingMappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<SearchPage, SearchPageDTO>()
            .Map(dest => dest.Url, src => src.Url!.Url)
            .Map(dest => dest.Number, src => src.Number);

        config.NewConfig<SearchPaging, SearchPagingDTO>()
            .MapWith(paging => new SearchPagingDTO
            {
                Pages = paging.Pages.Select(p => p.Adapt<SearchPageDTO>()).ToList(),
                FirstPage = paging.FirstPage.Adapt<SearchPageDTO>(),
                LastPage = paging.LastPage.Adapt<SearchPageDTO>(),
                NextPage = paging.NextPage.Adapt<SearchPageDTO>(),
                PreviousPage = paging.PreviousPage.Adapt<SearchPageDTO>()
            });
    }
}
