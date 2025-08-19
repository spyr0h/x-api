using System.Collections.Generic;
using XApi.Core.Page.Ports.Interfaces;
using XApi.Core.Suggestion.Enums;
using XApi.Core.Suggestion.Models;
using XApi.Core.Suggestion.Ports.Interfaces;
using XApi.Core.Videos.Models;

namespace XApi.Core.Suggestion.Services;

public class SuggestionService(
    ISuggestionProvider suggestionProvider, 
    IPageLinkProvider pageLinkProvider) : ISuggestionService
{
    private readonly int _suggestedMaxNumber = 30;
    private readonly int _suggestedPornstarMaxNumber = 10;
    private readonly int _suggestedSimilarMaxNumber = 20;
    private readonly double _percentageOfTags = 0.8;

    public async Task<SuggestionBox[]> ProvideSuggestions(Video video)
    {
        var suggestedVideos = (await suggestionProvider.ProvideSuggestedVideos(video))
            .Where(suggestedVideo => suggestedVideo.ID != video.ID)
            .Select(suggestedVideo => suggestedVideo with { Url = pageLinkProvider.ProvidePageLink(suggestedVideo)!.Url })
            .ToArray();

        if (suggestedVideos == null) return [];

        (List<Video> similarVideos, List<Video> similarPornstarVideos, List<Video> similarTagVideos, List<Video> similarCategoryVideos)
            = GetSplittedSuggestedVideos(video, suggestedVideos);

        var suggestionBoxes = new List<SuggestionBox>();

        if (similarPornstarVideos.Count != 0)
            suggestionBoxes.Add(ConstructPornstarBox(similarPornstarVideos, similarTagVideos.Count + similarCategoryVideos.Count));

        if (similarTagVideos.Count != 0 || similarCategoryVideos.Count != 0)
            suggestionBoxes.Add(
                ConstructSimilarBox(
                    similarVideos,
                    similarTagVideos, 
                    similarCategoryVideos, 
                    suggestionBoxes.FirstOrDefault()?.SuggestedVideos?.Length ?? 0));

        return [.. suggestionBoxes];
    }

    private SuggestionBox ConstructPornstarBox(List<Video> similarPornstarVideos, int similarCount)
    {
        // if there is not enough similar videos, we fill with pornstars
        var pornstarCount = similarCount > _suggestedSimilarMaxNumber ? _suggestedPornstarMaxNumber : _suggestedMaxNumber - similarCount;

        return new SuggestionBox
        {
            Title = "More from same pornstars",
            Order = 1,
            Category = SuggestionCategory.PornstarSuggestion,
            SuggestedVideos = similarPornstarVideos.Take(pornstarCount).ToArray()
        };
    }

    private SuggestionBox ConstructSimilarBox(
        List<Video> similarVideos, 
        List<Video> similarTagVideos, 
        List<Video> similarCategoryVideos, 
        int pornstarCount)
    {
        // Tags = 80%, Categories = 20%
        var countLeft = _suggestedMaxNumber - pornstarCount - similarVideos.Count;
        var tagMaxCount = (int)Math.Ceiling(countLeft * _percentageOfTags);
        var tagCount = similarTagVideos.Count;
        var catMaxCount = countLeft - tagMaxCount;
        var catCount = similarCategoryVideos.Count;

        var catFinalCount = (catCount, tagCount) switch
        {
            (_, _) when catCount < catMaxCount => catCount, // not enough categories
            (_, _) when tagCount > tagMaxCount => catMaxCount, // too much categories and tags
            (_, _) => countLeft - tagCount // too much categories but not enough tags, we fill
        };

        var tagFinalCount = countLeft - catFinalCount;

        return new SuggestionBox
        {
            Title = "Similar videos",
            Order = 2,
            Category = SuggestionCategory.SimilarSuggestion,
            SuggestedVideos = [
                .. similarVideos,
                .. similarTagVideos.Take(tagFinalCount),
                .. similarCategoryVideos.Take(catFinalCount)
            ]
        };
    }

    private (List<Video>, List<Video>, List<Video>, List<Video>) GetSplittedSuggestedVideos(Video video, Video[] suggestedVideos)
    {
        var similarVideos = new List<Video>();
        var similarPornstarVideos = new List<Video>();
        var similarTagVideos = new List<Video>();
        var similarCategoryVideos = new List<Video>();

        foreach (var suggestedVideo in suggestedVideos)
        {
            if (suggestedVideo.Pornstars.Any(video.Pornstars.Contains))
                similarPornstarVideos.Add(suggestedVideo);

            else if (suggestedVideo.Tags.Any(video.Tags.Contains)
                && suggestedVideo.Categories.Any(video.Categories.Contains))
                similarVideos.Add(suggestedVideo);

            else if (suggestedVideo.Tags.Any(video.Tags.Contains))
                similarTagVideos.Add(suggestedVideo);

            else if (suggestedVideo.Categories.Any(video.Categories.Contains))
                similarCategoryVideos.Add(suggestedVideo);
        }

        return (similarVideos, similarPornstarVideos, similarTagVideos, similarCategoryVideos);
    }
}
