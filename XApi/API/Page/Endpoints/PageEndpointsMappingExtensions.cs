using Mapster;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using XApi.API.Linkbox.DTO;
using XApi.API.Page.DTO;
using XApi.API.Paging.DTO;
using XApi.API.Search.Builder.Interfaces;
using XApi.API.Search.DTO;
using XApi.API.Seo.DTO;
using XApi.API.Suggestion.DTO;
using XApi.Core.Linkbox.Models;
using XApi.Core.Linkbox.Ports.Interfaces;
using XApi.Core.Page.Exceptions;
using XApi.Core.Page.Models;
using XApi.Core.Page.Ports.Interfaces;
using XApi.Core.Paging.Ports.Interfaces;
using XApi.Core.Search.Models;
using XApi.Core.Search.Ports.Interfaces;
using XApi.Core.Seo.Ports.Interfaces;
using XApi.Core.Suggestion.Ports.Interfaces;
using XApi.Core.Videos.Ports.Interfaces;

namespace XApi.API.Page.Endpoints;

public static class PageEndpointsMappingExtensions
{
    public static void MapPageEndpoints(this WebApplication webApplication)
    {
        webApplication.MapPost("/api/page/search/criteria", async (
            [FromBody] PageCriteriaDTO dto,
            IPageLinkProvider pageLinkProvider,
            ISearchService searchService,
            ISearchCriteriaBuilder searchCriteriaBuilder,
            IPagingService pagingService,
            ISeoService seoService,
            ILinkboxService linkboxService) =>
        {
            var searchCriteria = await searchCriteriaBuilder.BuildFrom(dto.SearchCriteriaDTO);
            if (pageLinkProvider.ProvidePageLink(searchCriteria)?.Url == null) return Results.BadRequest();
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
                if (searchCriteria == null) return Results.BadRequest();    
                return await GetSearchResult(searchCriteria, searchService, pagingService, seoService, linkboxService);
            }
            catch(RoutingException e)
            {
                Log.Warning(e.Message);
                return Results.BadRequest(e.Message);
            }
        })
        .WithName("url-serp-page")
        .WithOpenApi();

        webApplication.MapPost("/api/page/detail/url", async (
            [FromBody] PageLinkDTO dto,
            IPageRoutingService pageRoutingService,
            IVideoService videoService,
            ISeoService seoService,
            ILinkboxService linkboxService,
            ISuggestionService suggestionService) =>
        {
            try
            {
                var videoId = await pageRoutingService.RoutePageLinkToVideoId(dto.Adapt<PageLink>());
                if (videoId == null) return Results.BadRequest();
                var video = await videoService.ProvideVideoForId(videoId!.Value);
                if (video == null) return Results.BadRequest();

                return Results.Ok(new DetailPageResultDTO
                {
                    Video = video,
                    SeoData = seoService.ProvideSeoData(video).Adapt<SeoDataDTO>(),
                    Linkboxes = (await linkboxService.ProvideLinkboxes(video)).Adapt<LinkboxesDTO>(),
                    SuggestionBoxes = (await suggestionService.ProvideSuggestions(video))
                        .Select(suggestionBox => suggestionBox.Adapt<SuggestionBoxDTO>())
                        .ToArray()
                });
            }
            catch (RoutingException e)
            {
                Log.Warning(e.Message);
                return Results.BadRequest(e.Message);
            }
        })
        .WithName("url-detail-page")
        .WithOpenApi();
    }

    private static async Task<IResult> GetSearchResult(
        SearchCriteria searchCriteria,
        ISearchService searchService,
        IPagingService pagingService,
        ISeoService seoService,
        ILinkboxService linkboxService)
    {
        var searchResult = (searchCriteria.Categories.Count == 0
            && searchCriteria.Tags.Count == 0
            && searchCriteria.Pornstars.Count == 0) ? new SearchResult
            {
                Count = 0,
                GlobalCount = 0,
                Videos = []
            } : await searchService.SearchVideosByCriteria(searchCriteria);
        var searchPaging = await pagingService.CalculatePagingFromSearchData(searchCriteria, searchResult);
        var seoData = seoService.ProvideSeoData(searchCriteria);
        var linkboxes = await linkboxService.ProvideLinkboxes(searchCriteria);

        return Results.Ok(new SerpPageResultDTO
        {
            SearchCriteria = searchCriteria.Adapt<SearchCriteriaDTO>(),
            SearchResult = searchResult.Adapt<SearchResultDTO>(),
            SeoData = seoData.Adapt<SeoDataDTO>(),
            SearchPaging = searchPaging.Adapt<SearchPagingDTO>(),
            Linkboxes = linkboxes.Adapt<LinkboxesDTO>()
        });
    }
}
