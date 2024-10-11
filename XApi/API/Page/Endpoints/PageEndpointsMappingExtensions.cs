using Mapster;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using XApi.API.Linkbox.DTO;
using XApi.API.Page.DTO;
using XApi.API.Paging.DTO;
using XApi.API.Search.Builder.Interfaces;
using XApi.API.Search.DTO;
using XApi.API.Seo.DTO;
using XApi.Core.Linkbox.Ports.Interfaces;
using XApi.Core.Page.Exceptions;
using XApi.Core.Page.Models;
using XApi.Core.Page.Ports.Interfaces;
using XApi.Core.Paging.Ports.Interfaces;
using XApi.Core.Search.Models;
using XApi.Core.Search.Ports.Interfaces;
using XApi.Core.Seo.Ports.Interfaces;

namespace XApi.API.Page.Endpoints;

public static class PageEndpointsMappingExtensions
{
    public static void MapPageEndpoints(this WebApplication webApplication)
    {
        webApplication.MapPost("/api/page/search/criteria", async (
            [FromBody] PageCriteriaDTO dto,
            ISearchService searchService,
            ISearchCriteriaBuilder searchCriteriaBuilder,
            IPagingService pagingService,
            ISeoService seoService,
            ILinkboxService linkboxService) =>
        {
            var searchCriteria = await searchCriteriaBuilder.BuildFrom(dto.SearchCriteriaDTO);
            return await GetSearchResult(searchCriteria, searchService, pagingService, seoService, linkboxService);
        })
        .WithName("criteria-page")
        .WithOpenApi();

        webApplication.MapPost("/api/page/search/url", async (
            [FromBody] PageLinkDTO dto,
            IPageRoutingService pageRoutingService,
            ISearchService searchService,
            IPagingService pagingService,
            ISeoService seoService,
            ILinkboxService linkboxService) =>
        {
            try
            {
                var searchCriteria = await pageRoutingService.RoutePageLinkToCriteria(dto.Adapt<PageLink>());
                return await GetSearchResult(searchCriteria, searchService, pagingService, seoService, linkboxService);
            }
            catch(RoutingException e)
            {
                Log.Warning(e.Message);
                return Results.BadRequest(e.Message);
            }
        })
        .WithName("url-page")
        .WithOpenApi();
    }

    private static async Task<IResult> GetSearchResult(
        SearchCriteria searchCriteria,
        ISearchService searchService,
        IPagingService pagingService,
        ISeoService seoService,
        ILinkboxService linkboxService)
    {
        var searchResult = await searchService.SearchVideosByCriteria(searchCriteria);
        var searchPaging = await pagingService.CalculatePagingFromSearchData(searchCriteria, searchResult);
        var seoData = seoService.ProvideSeoData(searchCriteria);
        var linkboxes = linkboxService.ProvideLinkboxes(searchCriteria);

        return Results.Ok(new PageResultDTO
        {
            SearchCriteria = searchCriteria.Adapt<SearchCriteriaDTO>(),
            SearchResult = searchResult.Adapt<SearchResultDTO>(),
            SeoData = seoData.Adapt<SeoDataDTO>(),
            SearchPaging = searchPaging.Adapt<SearchPagingDTO>(),
            Linkboxes = linkboxes.Adapt<LinkboxesDTO>()
        });
    }
}
