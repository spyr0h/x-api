using XApi.Core.Categories.Ports.Interfaces;
using XApi.Core.Linkbox.Enums;
using XApi.Core.Linkbox.Models;
using XApi.Core.Linkbox.Ports.Interfaces;
using XApi.Core.Page.Ports.Interfaces;
using XApi.Core.Pornstars.Models;
using XApi.Core.Pornstars.Ports.Interfaces;
using XApi.Core.Search.Models;
using XApi.Core.Tags.Models;

namespace XApi.Core.Linkbox.Services;

public class LinkboxService(IPageLinkProvider pageLinkProvider, ICategoryService categoryService, IPornstarService pornstarService) : ILinkboxService
{
    public async Task<Models.Linkbox[]> ProvideLinkboxes(SearchCriteria criteria)
     => [
            new Models.Linkbox
            {
                Title = "Categories",
                Category = LinkboxCategory.CategoriesLinkbox,
                Order = 1,
                Links = await GetCategoriesLinks()
            },
            new Models.Linkbox
            {
                Title = "Pornstars",
                Category = LinkboxCategory.PornstarsLinkbox,
                Order = 2,
                Links = await GetPornstarsLinks()
            }
        ];

    private async Task<LinkboxLink[]> GetCategoriesLinks()
    {
        var categoriesList = await categoryService.ProvideAllCategories();
        return categoriesList
            .Where(category => category.Count != 0)
            .OrderByDescending(category => category.Count)
            .Select((category, i) => new LinkboxLink
            {
                Url = pageLinkProvider.ProvidePageLink(new SearchCriteria()
                {
                    Categories = [
                        category
                    ]
                })?.Url,
                Order = i,
                LinkText = $"{category.Value} ({category.Count})",
                Count = category.Count
            })
            .ToArray();
    }

    private async Task<LinkboxLink[]> GetPornstarsLinks()
    {
        var pornstarsList = await pornstarService.ProvideAllPornstars();
        return pornstarsList
            .Where(pornstar => pornstar.Count != 0)
            .OrderByDescending(pornstar => pornstar.Count)
            .Select((pornstar, i) => new LinkboxLink
            {
                Url = pageLinkProvider.ProvidePageLink(new SearchCriteria()
                {
                    Pornstars = [
                        pornstar
                    ]
                })?.Url,
                Order = i,
                LinkText = $"{pornstar.Value} ({pornstar.Count})",
                Count = pornstar.Count
            })
            .ToArray();
    }
}
