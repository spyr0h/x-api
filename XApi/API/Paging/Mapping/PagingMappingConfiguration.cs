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
            .Map(dest => dest.Number, src => src.Number)
            .Map(dest => dest.Selected, src => src.Selected);

        config.NewConfig<SearchPaging, SearchPagingDTO>()
            .MapWith(paging => new SearchPagingDTO
            {
                Pages = paging.Pages.Select(p => p.Adapt<SearchPageDTO>()).ToList(),
                NextPage = paging.NextPage.Adapt<SearchPageDTO>(),
                PreviousPage = paging.PreviousPage.Adapt<SearchPageDTO>()
            });
    }
}
