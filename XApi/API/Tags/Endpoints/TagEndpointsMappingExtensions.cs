using Mapster;
using Microsoft.AspNetCore.Mvc;
using XApi.API.Filter;
using XApi.API.Tags.DTO;
using XApi.Core.Tags.Models;
using XApi.Core.Tags.Ports.Interfaces;

namespace XApi.API.Tags.Endpoints;

public static class TagEndpointsMappingExtensions
{
    public static void MapTagEndpoints(this WebApplication webApplication)
        => webApplication.MapPost("/api/tag/autocomplete", async ([FromBody] TagAutocompleteDTO dto, ITagService tagService) =>
        {
            var foundTags = await tagService.Autocomplete(dto.Adapt<TagAutocomplete>());
            return Results.Ok(new TagsDTO
            {
                Tags = foundTags.Select(tag => tag.Adapt<TagDTO>()).ToList()
            });
        })
        .AddEndpointFilter<PrivateApiKeyAuthorizationFilter>()
        .WithName("tag-autocomplete")
        .WithOpenApi();
}
