using XApi.Common.Extensions;
using XApi.Core.Search.Models;
using XApi.Core.Seo.Builders.Interfaces;

namespace XApi.Core.Seo.Builders;

public class DescriptionBuilder : IDescriptionBuilder
{
    public string BuildFrom(SearchCriteria criteria)
    {
        var catRepresentation = string.Join(' ', criteria.Categories.Select(t => t.Value) ?? []).CapitalizeFirstLetter();
        catRepresentation = string.IsNullOrEmpty(catRepresentation) ? string.Empty : $"{catRepresentation} ";
        var tagRepresentation = string.Join(' ', criteria.Tags.Select(t => t.Value) ?? []).CapitalizeFirstLetter();
        tagRepresentation = string.IsNullOrEmpty(tagRepresentation) ? string.Empty : $"{tagRepresentation} ";
        var pornstarRepresentation = string.Join(", ", criteria.Pornstars.Select(p => p.Value?.CapitalizeFirstLetterOfEachWord() ?? "") ?? []);
        pornstarRepresentation = string.IsNullOrEmpty(pornstarRepresentation) ? string.Empty : $" of {pornstarRepresentation}";

        return $"{catRepresentation}{tagRepresentation}Porn Videos{pornstarRepresentation}";
    }
}
