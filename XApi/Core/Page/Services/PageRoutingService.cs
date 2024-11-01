using System.Text.RegularExpressions;
using XApi.Core.Page.Models;
using XApi.Core.Page.Ports.Interfaces;
using XApi.Core.Pornstars.Ports.Interfaces;
using XApi.Core.Search.Models;
using XApi.Core.Tags.Ports.Interfaces;
using XApi.Core.Page.Exceptions;
using XApi.Core.Categories.Ports.Interfaces;

namespace XApi.Core.Page.Services;

public class PageRoutingService(ITagService tagService, ICategoryService categoryService, IPornstarService pornstarService) : IPageRoutingService
{
    public async Task<SearchCriteria?> RoutePageLinkToCriteria(PageLink pageLink)
    {
        var (tags, pornstars, categories, page) = GetUrlParsedRawData(pageLink);

        if (pageLink.Url!.Contains("/videos/all"))
            return new()
            {
                Paging = new()
                {
                    PageIndex = page,
                    ResultsPerPage = 25
                }
            };

        var foundCategories = await Task.WhenAll(categories
            .Select(async category => (await categoryService.ProvideCategoryForValue(category.Replace("-", " ")))
                ?? throw new RoutingException($"Category does not exists : {category}.")));

        var foundTags = await Task.WhenAll(tags
            .Select(async tag => (await tagService.ProvideTagForValue(tag.Replace("-", " ")))
                ?? throw new RoutingException($"Tag does not exists : {tag}.")));

        var foundPornstars = await Task.WhenAll(pornstars
            .Select(async pornstar => (await pornstarService.ProvidePornstarForValue(pornstar.Replace("-", " ")))
                ?? throw new RoutingException($"Pornstar does not exists : {pornstar}.")));

        return new()
        {
            Categories = [.. foundCategories ],
            Tags = [.. foundTags],
            Pornstars = [.. foundPornstars],
            Paging = new SearchPagingSpecs
            {
                PageIndex = page,
                ResultsPerPage = 25
            }
        };
    }

    public async Task<int?> RoutePageLinkToVideoId(PageLink pageLink)
    {
        if (!Uri.IsWellFormedUriString(pageLink.Url, UriKind.RelativeOrAbsolute)) return default;
        var splitted = pageLink.Url.Split("/").Last().Split("-");

        if (int.TryParse(splitted[0], out int id))
            return id;

        return null;
    }

    private (string[], string[], string[], int) GetUrlParsedRawData(PageLink pageLink)
    {
        //string pattern = @"(?:tags=([^&\n]*))|(?:pornstars=([^&\n]*))|(?:page=(\d+))"; IF TECHNICAL URL
        string pattern = @"(?:videos\/all)|(?:videos\/tags\/([^\/]+))|(?:videos\/pornstars\/([^\/]+))|(?:videos\/categories\/([^\/]+))|(?:\/(\d+))";

        var matches = Regex.Matches(pageLink.Url ?? "", pattern);

        if (!matches.Any())
            throw new RoutingException($"Impossible to root url : {pageLink.Url}.");

        string[] categories = [];
        string[] tags = [];
        string[] pornstars = [];
        int page = 1;

        foreach (Match match in matches)
        {
            if (!string.IsNullOrEmpty(match.Groups[1].Value))
            {
                tags = [ match.Groups[1].Value ];
            }
            if (!string.IsNullOrEmpty(match.Groups[2].Value))
            {
                pornstars = [ match.Groups[2].Value ];
            }
            if (!string.IsNullOrEmpty(match.Groups[3].Value))
            {
                categories = [ match.Groups[3].Value ];
            }
            if (!string.IsNullOrEmpty(match.Groups[4].Value))
            {
                page = int.Parse(match.Groups[4].Value);
            }
        }

        return (tags, pornstars, categories, page);
    }
}
