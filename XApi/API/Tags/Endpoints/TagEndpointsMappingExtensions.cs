using Mapster;
using Microsoft.AspNetCore.Mvc;
using XApi.API.Tags.DTO;
using XApi.Core.Tags.Models;
using XApi.Core.Tags.Services.Interfaces;

namespace XApi.API.Tags.Endpoints;

public static class TagEndpointsMappingExtensions
{
    public static void MapTagsEndpoints(this WebApplication webApplication)
        => webApplication.MapPost("/api/tags/autocomplete", ([FromBody] TagAutocompleteDTO dto, ITagService tagService) =>
        {
            var foundTags = tagService.Autocomplete(dto.Adapt<TagAutocomplete>());
            return Results.Ok(new TagsDTO
            {
                Tags = foundTags.Select(tag => tag.Adapt<TagDTO>()).ToList()
            });
        })
        .WithName("autocomplete")
        .WithOpenApi();
}
