using XApi.Core.Page.Ports.Interfaces;
using XApi.Core.Search.Models;
using XApi.Core.Search.Ports.Interfaces;

namespace XApi.Core.Search.Services;

public class SearchService(
    ISearchProvider searchProvider,
    IPageLinkProvider pageLinkProvider) : ISearchService
{
    public async Task<SearchResult> SearchVideosByCriteria(SearchCriteria searchCriteria)
    {
        var searchResult = await searchProvider.SearchVideosByCriteria(searchCriteria);
        return searchResult with
        {
            Videos = searchResult.Videos
                .Select(video => video with
                {
                    Url = pageLinkProvider.ProvidePageLink(video)!.Url
                })
                .ToList()
        };
    }
}
