using Mapster;
using Microsoft.AspNetCore.Mvc;
using XApi.API.Page.DTO;
using XApi.API.Paging.DTO;
using XApi.API.Search.Builder.Interfaces;
using XApi.API.Search.DTO;
using XApi.API.Seo.DTO;
using XApi.Core.Paging.Ports.Interfaces;
using XApi.Core.Search.Ports.Interfaces;
using XApi.Core.Seo.Ports.Interfaces;

namespace XApi.API.Page.Endpoints;

public static class PageEndpointsMappingExtensions
{
    public static void MapPageEndpoints(this WebApplication webApplication)
        => webApplication.MapPost("/api/page/criteria", async (
            [FromBody] PageCriteriaDTO dto, 
            ISearchService searchService,
            ISearchCriteriaBuilder searchCriteriaBuilder,
            IPagingService pagingService,
            ISeoService seoService) =>
        {
            var searchCriteria = await searchCriteriaBuilder.BuildFrom(dto.SearchCriteriaDTO);
            var searchResult = await searchService.SearchVideosByCriteria(searchCriteria);
            var searchPaging = await pagingService.CalculatePagingFromSearchData(searchCriteria, searchResult);
            var seoData = seoService.ProvideSeoData(searchCriteria);
            return Results.Ok(new PageResultDTO
            {
                SearchResult = searchResult.Adapt<SearchResultDTO>(),
                SeoData = seoData.Adapt<SeoDataDTO>(),
                SearchPaging = searchPaging.Adapt<SearchPagingDTO>()
            });
        })
        .WithName("criteria-page")
        .WithOpenApi();
}
