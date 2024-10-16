using XApi.Core.Page.Ports.Interfaces;
using XApi.Core.Paging.Models;
using XApi.Core.Paging.Ports.Interfaces;
using XApi.Core.Search.Models;

namespace XApi.Core.Paging.Services;

public class PagingService(IPageLinkProvider pageLinkProvider) : IPagingService
{
    public async Task<SearchPaging> CalculatePagingFromSearchData(SearchCriteria searchCriteria, SearchResult searchResult)
    {
        int actualPage = searchCriteria.Paging.PageIndex;
        int maxPage = (int)Math.Ceiling((double)searchResult.GlobalCount / searchCriteria.Paging.ResultsPerPage);
        maxPage = maxPage == 0 ? 1 : maxPage;

        var firstPageCriteria = actualPage == 1
            ? null
            : searchCriteria with
            {
                Paging = searchCriteria.Paging with { PageIndex = 1 }
            };

        var lastPageCriteria = actualPage == maxPage
            ? null
            : searchCriteria with
            {
                Paging = searchCriteria.Paging with { PageIndex = maxPage }
            };

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

        var pages = GetPages(actualPage, maxPage)
            .ToArray()
            .Select(page => searchCriteria with
            {
                Paging = searchCriteria.Paging with { PageIndex = page }
            });

        return new SearchPaging()
        {
            FirstPage = BuildWith(firstPageCriteria),
            LastPage = BuildWith(lastPageCriteria),
            PreviousPage = BuildWith(previousPage),
            NextPage = BuildWith(nextPage),
            Pages = pages
                .Select(BuildWith)
                .Where(page => page != null)
                .Cast<SearchPage>()
                .ToList()
        };
    }

    private SearchPage? BuildWith(SearchCriteria? searchCriteria)
    {
        if (searchCriteria == null) return null;

        var pageUrl = pageLinkProvider.ProvidePageLink(searchCriteria);
        return new()
        {
            Url = pageUrl,
            Number = searchCriteria.Paging.PageIndex
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
