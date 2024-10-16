using System.Text.RegularExpressions;
using XApi.Core.Page.Models;
using XApi.Core.Page.Ports.Interfaces;
using XApi.Core.Pornstars.Ports.Interfaces;
using XApi.Core.Search.Models;
using XApi.Core.Tags.Ports.Interfaces;
using XApi.Core.Page.Exceptions;

namespace XApi.Core.Page.Services;

public class PageRoutingService(ITagService tagService, IPornstarService pornstarService) : IPageRoutingService
{
    public async Task<SearchCriteria> RoutePageLinkToCriteria(PageLink pageLink)
    {
        var (tags, pornstars, page) = GetUrlParsedRawData(pageLink);

        var foundTags = await Task.WhenAll(tags
            .Select(async tag => (await tagService.ProvideTagForValue(tag.Replace("+", " ")))
                ?? throw new RoutingException($"Tag does not exists : {tag}.")));

        var foundPornstars = await Task.WhenAll(pornstars
            .Select(async pornstar => (await pornstarService.ProvidePornstarForValue(pornstar.Replace("+", " ")))
                ?? throw new RoutingException($"Pornstar does not exists : {pornstar}.")));

        return new()
        {
            Tags = [.. foundTags],
            Pornstars = [.. foundPornstars],
            Paging = new SearchPagingSpecs
            {
                PageIndex = page,
                ResultsPerPage = 25
            }
        };
    }

    public (string[], string[], int) GetUrlParsedRawData(PageLink pageLink)
    {
        string pattern = @"(?:tags=([^&\n]*))|(?:pornstars=([^&\n]*))|(?:page=(\d+))";
        var matches = Regex.Matches(pageLink.Url ?? "", pattern);

        string[] tags = [];
        string[] pornstars = [];
        int page = 1;

        foreach (Match match in matches)
        {
            if (!string.IsNullOrEmpty(match.Groups[1].Value))
            {
                tags = match.Groups[1].Value.Split(',');
            }
            if (!string.IsNullOrEmpty(match.Groups[2].Value))
            {
                pornstars = match.Groups[2].Value.Split(',');
            }
            if (!string.IsNullOrEmpty(match.Groups[3].Value))
            {
                page = int.Parse(match.Groups[3].Value);
            }
        }

        return (tags, pornstars, page);

        throw new RoutingException($"Impossible to root url : {pageLink.Url}.");
    }
}
