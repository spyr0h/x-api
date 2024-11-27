using XApi.Common.Extensions;
using XApi.Core.Search.Models;
using XApi.Core.Seo.Builders.Interfaces;

namespace XApi.Core.Seo.Builders;

public class TitleBuilder : ITitleBuilder
{
    public string BuildFrom(SearchCriteria criteria, SearchResult searchResult)
    {
        if (criteria.Categories.Count == 1
            && !string.IsNullOrEmpty(criteria.Categories.First().Title)
            && criteria.Tags.Count == 0
            && criteria.Pornstars.Count == 0)
        {
            return string.Format(criteria.Categories.First().Title!.Replace("{placeholder}", searchResult.GlobalCount.ToString()));
        }

        return FallBack(criteria, searchResult);
    }

    private string FallBack(SearchCriteria criteria, SearchResult searchResult)
    {
        var catRepresentation = string.Join(' ', criteria.Categories.Select(t => t.Value) ?? []).CapitalizeFirstLetter();
        catRepresentation = string.IsNullOrEmpty(catRepresentation) ? string.Empty : $"{catRepresentation} ";
        var tagRepresentation = string.Join(' ', criteria.Tags.Select(t => t.Value) ?? []).CapitalizeFirstLetter();
        tagRepresentation = string.IsNullOrEmpty(tagRepresentation) ? string.Empty : $"{tagRepresentation} ";
        var pornstarRepresentation = string.Join(", ", criteria.Pornstars.Select(p => p.Value?.CapitalizeFirstLetterOfEachWord() ?? "") ?? []);
        pornstarRepresentation = string.IsNullOrEmpty(pornstarRepresentation) ? string.Empty : $" of {pornstarRepresentation}";

        return $@"{(searchResult.GlobalCount == 0 ? string.Empty : searchResult.GlobalCount)}
            {catRepresentation}{tagRepresentation}
                Porn Videos{pornstarRepresentation}";
    }
}
