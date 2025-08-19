using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;
using XApi.Core.Categories.Models;
using XApi.Core.Page.Models;
using XApi.Core.Page.Ports.Interfaces;
using XApi.Core.Pornstars.Models;
using XApi.Core.Search.Enums;
using XApi.Core.Search.Models;
using XApi.Core.Tags.Models;
using XApi.Core.Videos.Models;

namespace XApi.Core.Page.Services;

public class PageLinkProvider : IPageLinkProvider
{
    public PageLink? ProvidePageLink(SearchCriteria criteria)
    {
        var pageLink = criteria switch
        {
            { Terms: not null } => new PageLink { Url = $"/videos/search?terms={string.Join("+", criteria.Terms.Split(" "))}" },
            { Pornstars: [], Categories: [] } when criteria.Tags.Count == 1 => GenerateTagUrl(criteria.Tags),
            { Pornstars: [], Tags: [] } when criteria.Categories.Count == 1 => GenerateCategoryUrl(criteria.Categories),
            { Categories: [], Tags: [] } when criteria.Pornstars.Count == 1 => GeneratePornstarUrl(criteria.Pornstars),
            { Categories: [], Tags: [], Pornstars: [] } => new PageLink { Url = "/videos/all" },
            { Paging.SearchOrder: SearchOrder.Clicks } => new PageLink { Url = "/videos/best" },
            _ => new PageLink { Url = "/videos/all" }
        };

        if (pageLink == null) return null;

        var page = criteria.Paging.PageIndex == 1 
            ? "" 
            : criteria.Terms != null 
                ? $"&page={criteria.Paging.PageIndex}" 
                : $"/{criteria.Paging.PageIndex}";

        return pageLink with { Url = $"{pageLink.Url}{page}" };
    }

    public PageLink? ProvidePageLink(Video video)
        => new()
        {
            Url = BuildVideoUrl(video.ID, video.Title)
        };

    private PageLink GeneratePornstarUrl(List<Pornstar> pornstars)
        => new()
        {
            Url = $"/videos/pornstars/{Slugify(pornstars.First().Value!)}"
        };

    private PageLink GenerateCategoryUrl(List<Category> categories)
        => new()
        {
            Url = $"/videos/categories/{Slugify(categories.First().Value!)}"
        };

    private PageLink GenerateTagUrl(List<Tag> tags)
        => new()
        {
            Url = $"/videos/tags/{Slugify(tags.First().Value!)}"
        };

    private string Slugify(string value)
    {
        if (value == null) return "error";
        var splitted = value.Split(' ').Select(split => split.ToLower());
        return string.Join("-", splitted);
    }

    private string BuildVideoUrl(int id, string? title)
    {
        if (string.IsNullOrEmpty(title))
            return $"/video/{id}-unknown-title";
        return $"/video/{id}-{GenerateUrlPart(title)}";
    }

    private string GenerateUrlPart(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return string.Empty;

        string normalized = title.Normalize(NormalizationForm.FormD);

        var stringBuilder = new StringBuilder();
        foreach (char c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        string result = stringBuilder.ToString();

        result = result.ToLowerInvariant();

        result = Regex.Replace(result, @"[^a-z0-9\s]", "");

        result = Regex.Replace(result, @"\s+", "-");

        return result;
    }
}