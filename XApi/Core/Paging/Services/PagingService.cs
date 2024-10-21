using XApi.Core.Page.Ports.Interfaces;
using XApi.Core.Paging.Models;
using XApi.Core.Paging.Ports.Interfaces;
using XApi.Core.Search.Models;

namespace XApi.Core.Paging.Services;

public class PagingService(IPageLinkProvider pageLinkProvider) : IPagingService
{
    public async Task<SearchPaging> CalculatePagingFromSearchData(SearchCriteria searchCriteria, SearchResult searchResult)
    {
        var pagesCriteria = new List<SearchCriteria>();
        int actualPage = searchCriteria.Paging.PageIndex;
        int maxPage = (int)Math.Ceiling((double)searchResult.GlobalCount / searchCriteria.Paging.ResultsPerPage);
        maxPage = maxPage == 0 ? 1 : maxPage;

        pagesCriteria.Add(searchCriteria with
        {
            Paging = searchCriteria.Paging with { PageIndex = 1 }
        });

        pagesCriteria.AddRange(GetPages(actualPage, maxPage)
            .ToArray()
            .Select(page => searchCriteria with
            {
                Paging = searchCriteria.Paging with { PageIndex = page }
            }));

        if (maxPage > 1)
            pagesCriteria.Add(searchCriteria with
            {
                Paging = searchCriteria.Paging with { PageIndex = maxPage }
            });

        if (!pagesCriteria.Any(p => p.Paging.PageIndex == actualPage))
            pagesCriteria.Add(searchCriteria);

        var previousPage = actualPage == 1
            ? null
            : searchCriteria with
            {
                Paging = searchCriteria.Paging with { PageIndex = actualPage - 1 }
            };

        var nextPage = actualPage == maxPage
            ? null
            : searchCriteria with
            {
                Paging = searchCriteria.Paging with { PageIndex = actualPage + 1 }
            };

        

        return new SearchPaging()
        {
            PreviousPage = BuildWith(previousPage, actualPage),
            NextPage = BuildWith(nextPage, actualPage),
            Pages = pagesCriteria
                .OrderBy(page => page.Paging.PageIndex)
                .Select(page => BuildWith(page, actualPage))
                .Where(page => page != null)
                .Cast<SearchPage>()
                .ToList()
        };
    }

    private SearchPage? BuildWith(SearchCriteria? searchCriteria, int actualPage)
    {
        if (searchCriteria == null) return null;

        var pageUrl = pageLinkProvider.ProvidePageLink(searchCriteria);

        if (pageUrl?.Url == null) return null;

        return new()
        {
            Url = pageUrl,
            Number = searchCriteria.Paging.PageIndex,
            Selected = searchCriteria.Paging.PageIndex == actualPage
        };
    }

    public int[] GetPages(int actualPage, int maxPage)
    {
        if (actualPage >= maxPage)
            return [];

        int count = Math.Min(10, maxPage - actualPage - 1);

        return Enumerable.Range(actualPage + 1, count).ToArray();
    }
}
