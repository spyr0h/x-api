using Mapster;
using Microsoft.AspNetCore.Mvc;
using XApi.API.Pornstars.DTO;
using XApi.Core.Pornstars.Models;
using XApi.Core.Pornstars.Ports.Interfaces;

namespace XApi.API.Pornstars.Endpoints;

public static class PornstarEndpointsMappingExtensions
{
    public static void MapPornstarEndpoints(this WebApplication webApplication)
        => webApplication.MapPost("/api/pornstar/autocomplete", async ([FromBody] PornstarAutocompleteDTO dto, IPornstarService pornstarService) =>
        {
            var foundPornstars = await pornstarService.Autocomplete(dto.Adapt<PornstarAutocomplete>());
            return Results.Ok(new PornstarsDTO
            {
                Pornstars = foundPornstars.Select(pornstar => pornstar.Adapt<PornstarDTO>()).ToList()
            });
        })
        .WithName("pornstar-autocomplete")
        .WithOpenApi();
}
