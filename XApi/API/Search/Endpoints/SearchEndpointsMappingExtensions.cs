using Mapster;
using Microsoft.AspNetCore.Mvc;
using XApi.API.Search.Builder.Interfaces;
using XApi.API.Search.DTO;
using XApi.Core.Search.Ports.Interfaces;

namespace XApi.API.Search.Endpoints;

public static class SearchEndpointsMappingExtensions
{
    public static void MapSearchEndpoints(this WebApplication webApplication)
        => webApplication.MapPost("/api/search/criteria", async (
            [FromBody] SearchCriteriaDTO dto, 
            ISearchService searchService, 
            ISearchCriteriaBuilder searchCriteriaBuilder) =>
        {
            var searchCriteria = await searchCriteriaBuilder.BuildFrom(dto);
            var searchResult = await searchService.SearchVideosByCriteria(searchCriteria);
            return Results.Ok(searchResult.Adapt<SearchResultDTO>());
        })
        .WithName("criteria-search")
        .WithOpenApi();
}
