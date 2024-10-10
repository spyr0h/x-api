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
        int pageNumber = (int)Math.Ceiling((double)searchResult.Count / searchCriteria.Paging.ResultsPerPage);

        var firstPageCriteria = actualPage == 1
            ? searchCriteria
            : searchCriteria with
            {
                Paging = searchCriteria.Paging with { PageIndex = 1 }
            };

        var lastPageCriteria = actualPage == pageNumber
            ? searchCriteria
            : searchCriteria with
            {
                Paging = searchCriteria.Paging with { PageIndex = pageNumber }
            };

        var previousPage = actualPage == 1
            ? null
            : searchCriteria with
            {
                Paging = searchCriteria.Paging with { PageIndex = actualPage - 1 }
            };

        var nextPage = actualPage == pageNumber
            ? null
            : searchCriteria with
            {
                Paging = searchCriteria.Paging with { PageIndex = actualPage + 1 }
            };

        var pages = Enumerable
            .Range(actualPage + 1, 10)
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
}
