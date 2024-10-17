using Mapster;
using Microsoft.AspNetCore.Mvc;
using XApi.API.Autocomplete.DTO;
using XApi.API.Tags.DTO;
using XApi.Core.Autocomplete.Enums;
using XApi.Core.Page.Ports.Interfaces;
using XApi.Core.Pornstars.Models;
using XApi.Core.Pornstars.Ports.Interfaces;
using XApi.Core.Tags.Models;
using XApi.Core.Tags.Ports.Interfaces;

namespace XApi.API.Autocomplete.Endpoints;

public static class AutocompleteEndpointsMappingExtensions
{
    public static void MapAutocompleteEndpoints(this WebApplication webApplication)
        => webApplication.MapPost("/api/full/autocomplete", async (
            [FromBody] FullAutocompleteDTO dto, 
            ITagService tagService, 
            IPornstarService pornstarService,
            IPageLinkProvider pageLinkProvider) =>
        {

            var tags = tagService.Autocomplete(dto.Adapt<TagAutocomplete>());
            var pornstars = pornstarService.Autocomplete(dto.Adapt<PornstarAutocomplete>());

            await Task.WhenAll([tags, pornstars]);

            List<SuggestionDTO> suggestions =
            [
                .. tags.Result.Select(tag => new SuggestionDTO
                { 
                    Value = tag.Value,
                    Type = SuggestionType.Tag,
                    SearchUrl = pageLinkProvider.ProvidePageLink(new() { Tags = [ tag ] }).Url ?? string.Empty
                }),
                .. pornstars.Result.Select(pornstar => new SuggestionDTO
                {
                    Value = pornstar.Value,
                    Type = SuggestionType.Pornstar,
                    SearchUrl = pageLinkProvider.ProvidePageLink(new() { Pornstars = [pornstar] }).Url ?? string.Empty
                }),
            ];

            return Results.Ok(new SuggestionsListDTO
            {
                Suggestions = suggestions
                    .Where(suggestion => !string.IsNullOrEmpty(suggestion.Value) && !string.IsNullOrEmpty(suggestion.SearchUrl))
                    .ToArray()
            });
        })
        .WithName("full-autocomplete")
        .WithOpenApi();
}
