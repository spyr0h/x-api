using XApi.Core.Linkbox.Enums;
using XApi.Core.Linkbox.Models;
using XApi.Core.Linkbox.Ports.Interfaces;
using XApi.Core.Page.Ports.Interfaces;
using XApi.Core.Pornstars.Models;
using XApi.Core.Search.Models;
using XApi.Core.Tags.Models;

namespace XApi.Core.Linkbox.Services;

public class LinkboxService(IPageLinkProvider pageLinkProvider) : ILinkboxService
{
    public Models.Linkbox[] ProvideLinkboxes(SearchCriteria criteria)
    {
        return
        [
            new Models.Linkbox
            {
                Title = "Other Tags",
                Category = LinkboxCategory.TagsLinkbox,
                Order = 1,
                Links = [
                    new LinkboxLink
                    {
                        Url = pageLinkProvider.ProvidePageLink(new SearchCriteria()
                        {
                            Tags = [
                                new Tag
                                {
                                    ID = 1, 
                                    Value = "bdsm"
                                }
                            ]
                        })?.Url,
                        Order = 1,
                        LinkText = "BDSM",
                        Count = 234
                    },
                    new LinkboxLink
                    {
                        Url = pageLinkProvider.ProvidePageLink(new SearchCriteria()
                        {
                            Tags = [
                                new Tag
                                {
                                    ID = 2,
                                    Value = "bukkake"
                                }
                            ]
                        })?.Url,
                        Order = 2,
                        LinkText = "Bukkake Interracial",
                        Count = 13020
                    }
                ]
            },
            new Models.Linkbox
            {
                Title = "Other pornstars",
                Category = LinkboxCategory.TagsLinkbox,
                Order = 2,
                Links = [
                    new LinkboxLink
                    {
                        Url = pageLinkProvider.ProvidePageLink(new SearchCriteria()
                        {
                            Pornstars = [
                                new Pornstar
                                {
                                    ID = 1,
                                    Value = "clara morgane"
                                }
                            ]
                        })?.Url,
                        Order = 1,
                        LinkText = "Clara Morgane",
                        Count = 1339
                    },
                    new LinkboxLink
                    {
                        Url = pageLinkProvider.ProvidePageLink(new SearchCriteria()
                        {
                            Pornstars = [
                                new Pornstar
                                {
                                    ID = 2,
                                    Value = "riley reid"
                                }
                            ]
                        })?.Url,
                        Order = 2,
                        LinkText = "Riley Reid",
                        Count = 213
                    }
                ]
            }
        ];
    }
}
