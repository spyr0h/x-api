using Mapster;
using Microsoft.AspNetCore.Mvc;
using XApi.Adapters.Mysql.Categories.Models;
using XApi.API.Autocomplete.DTO;
using XApi.API.Filter;
using XApi.API.Tags.DTO;
using XApi.Core.Autocomplete.Enums;
using XApi.Core.Categories.Models;
using XApi.Core.Categories.Ports.Interfaces;
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
            ICategoryService categoryService, 
            ITagService tagService, 
            IPornstarService pornstarService,
            IPageLinkProvider pageLinkProvider) =>
        {

            var categories = categoryService.Autocomplete(dto.Adapt<CategoryAutocomplete>());
            var tags = tagService.Autocomplete(dto.Adapt<TagAutocomplete>());
            var pornstars = pornstarService.Autocomplete(dto.Adapt<PornstarAutocomplete>());

            await Task.WhenAll([categories, tags, pornstars]);

            List<SuggestionDTO> suggestions =
            [
                ..categories.Result.OrderByDescending(c => c.Count).ThenBy(c => c.Value).Select(category => new SuggestionDTO
                {
                    Value = category.Count == 0 ? category.Value : $"{category.Value} ({category.Count})",
                    Type = SuggestionType.Category,
                    SearchUrl = pageLinkProvider.ProvidePageLink(new() { Categories = [ category ] })?.Url ?? string.Empty
                }).Take(3),
                .. tags.Result.OrderByDescending(t => t.Count).ThenBy(t => t.Value).Select(tag => new SuggestionDTO
                { 
                    Value = tag.Count == 0 ? tag.Value : $"{tag.Value} ({tag.Count})",
                    Type = SuggestionType.Tag,
                    SearchUrl = pageLinkProvider.ProvidePageLink(new() { Tags = [ tag ] })?.Url ?? string.Empty
                }).Take(3),
                .. pornstars.Result.OrderByDescending(p => p.Count).ThenBy(p => p.Value).Select(pornstar => new SuggestionDTO
                {
                    Value = pornstar.Count == 0 ? pornstar.Value : $"{pornstar.Value} ({pornstar.Count})",
                    Type = SuggestionType.Pornstar,
                    SearchUrl = pageLinkProvider.ProvidePageLink(new() { Pornstars = [pornstar] })?.Url ?? string.Empty
                }).Take(3),
            ];

            return Results.Ok(new SuggestionsListDTO
            {
                Suggestions = suggestions
                    .Where(suggestion => !string.IsNullOrEmpty(suggestion.Value) && !string.IsNullOrEmpty(suggestion.SearchUrl))
                    .ToArray()
            });
        })
        .AddEndpointFilter<PublicApiKeyAuthorizationFilter>()
        .WithName("full-autocomplete")
        .WithOpenApi();
}
