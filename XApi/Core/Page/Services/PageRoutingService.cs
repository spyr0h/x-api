using System.Text.RegularExpressions;
using XApi.Core.Page.Models;
using XApi.Core.Page.Ports.Interfaces;
using XApi.Core.Pornstars.Ports.Interfaces;
using XApi.Core.Search.Models;
using XApi.Core.Tags.Ports.Interfaces;
using XApi.Core.Page.Exceptions;
using XApi.Core.Categories.Ports.Interfaces;
using XApi.Core.Search.Enums;

namespace XApi.Core.Page.Services;

public class PageRoutingService(ITagService tagService, ICategoryService categoryService, IPornstarService pornstarService) : IPageRoutingService
{
    public async Task<SearchCriteria?> RoutePageLinkToCriteria(PageLink pageLink)
    {
        var (tags, pornstars, categories, page, terms) = GetUrlParsedRawData(pageLink);

        if (terms != null)
            return await ConstructFromTechnicalUrl(terms, page);
        else
        {
            return await ConstructFromStandardUrl(pageLink.Url!, tags, pornstars, categories, page);
        }
    }

    public async Task<int?> RoutePageLinkToVideoId(PageLink pageLink)
    {
        if (!Uri.IsWellFormedUriString(pageLink.Url, UriKind.RelativeOrAbsolute)) return default;
        var splitted = pageLink.Url.Split("/").Last().Split("-");

        if (int.TryParse(splitted[0], out int id))
            return id;

        return null;
    }

    private async Task<SearchCriteria> ConstructFromStandardUrl(
        string url, 
        string[] tags,
        string[] pornstars, 
        string[] categories, 
        int page)
    {
        if (url.Contains("/videos/all"))
            return new()
            {
                Paging = new()
                {
                    PageIndex = page,
                    ResultsPerPage = 25
                }
            };

        if (url.Contains("/videos/best"))
            return new()
            {
                Paging = new()
                {
                    PageIndex = page,
                    SearchOrder = SearchOrder.Clicks,
                    ResultsPerPage = 25
                }
            };

        var categoryTask = Task.WhenAll(categories
            .Select(async category => (await categoryService.ProvideCategoryForValue(category.Replace("-", " ")))
        ?? throw new RoutingException($"Category does not exist: {category}.")));

        var tagTask = Task.WhenAll(tags
            .Select(async tag => (await tagService.ProvideTagForValue(tag.Replace("-", " ")))
                ?? throw new RoutingException($"Tag does not exist: {tag}.")));

        var pornstarTask = Task.WhenAll(pornstars
            .Select(async pornstar => (await pornstarService.ProvidePornstarForValue(pornstar.Replace("-", " ")))
                ?? throw new RoutingException($"Pornstar does not exist: {pornstar}.")));

        await Task.WhenAll(categoryTask, tagTask, pornstarTask);

        var foundCategories = await categoryTask;
        var foundTags = await tagTask;
        var foundPornstars = await pornstarTask;

        return new()
        {
            Categories = [.. foundCategories],
            Tags = [.. foundTags],
            Pornstars = [.. foundPornstars],
            Paging = new SearchPagingSpecs
            {
                PageIndex = page,
                ResultsPerPage = 25
            }
        };
    }

    private async Task<SearchCriteria?> ConstructFromTechnicalUrl(string terms, int page)
    {
        var categoryTask = categoryService.ProvideCategoriesForTerms(terms);
        var tagTask = tagService.ProvideTagsForTerms(terms);
        var pornstarTask = pornstarService.ProvidePornstarsForTerms(terms);

        await Task.WhenAll(categoryTask, tagTask, pornstarTask);

        var foundCategories = await categoryTask;
        var foundTags = await tagTask;
        var foundPornstars = await pornstarTask;

        var crit = new SearchCriteria()
        {
            Terms = terms,
            Categories = [.. foundCategories],
            Tags = [.. foundTags],
            Pornstars = [.. foundPornstars],
            Paging = new SearchPagingSpecs
            {
                PageIndex = page,
                ResultsPerPage = 25
            }
        };

        return crit;
    }


    private (string[], string[], string[], int, string?) GetUrlParsedRawData(PageLink pageLink)
    {
        var url = pageLink.Url ?? "";

        return ParseTechnicalUrl(url) 
            ?? ParseStandardUrl(url) 
            ?? throw new RoutingException($"Impossible to root url : {url}.");
    }

    private (string[], string[], string[], int, string?)? ParseTechnicalUrl(string url)
    {
        string technicalPattern = @"videos\/search\?terms=([^&\n]*)(?:&page=(\d+))?";
        var technicalMatch = Regex.Match(url, technicalPattern);

        if (technicalMatch.Success)
        {
            string searchTerms = technicalMatch.Groups[1].Value.Replace("+", " ");
            int page = !string.IsNullOrEmpty(technicalMatch.Groups[2].Value)
                ? int.Parse(technicalMatch.Groups[2].Value)
                : 1;

            return ([], [], [], page, searchTerms);
        }

        return null;
    }

    private (string[], string[], string[], int, string?)? ParseStandardUrl(string url)
    {
        string pattern = @"(?:videos\/all)|(?:videos\/best)|(?:videos\/tags\/([^\/]+))|(?:videos\/pornstars\/([^\/]+))|(?:videos\/categories\/([^\/]+))|(?:\/(\d+))";
        var matches = Regex.Matches(url, pattern);

        if (matches.Count == 0)
        {
            return null;
        }

        string[] categories = [];
        string[] tags = [];
        string[] pornstars = [];
        int page = 1;

        foreach (Match match in matches)
        {
            if (!string.IsNullOrEmpty(match.Groups[1].Value))
            {
                tags = [match.Groups[1].Value];
            }
            if (!string.IsNullOrEmpty(match.Groups[2].Value))
            {
                pornstars = [match.Groups[2].Value];
            }
            if (!string.IsNullOrEmpty(match.Groups[3].Value))
            {
                categories = [match.Groups[3].Value];
            }
            if (!string.IsNullOrEmpty(match.Groups[4].Value))
            {
                page = int.Parse(match.Groups[4].Value);
            }
        }

        return (tags, pornstars, categories, page, null);
    }

}
