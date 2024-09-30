using Dapper;
using Mapster;
using MySql.Data.MySqlClient;
using System.Data;
using XApi.Adapters.Mysql.Videos.Models;
using XApi.Core.Search.Models;
using XApi.Core.Search.Ports.Interfaces;

namespace XApi.Adapters.Mysql.Search.Adapters;

public class SearchProvider : ISearchProvider
{
    private readonly string _connectionString;
    private readonly string _query = @"
        SELECT 
            v.ID AS VideoID,
            v.Title,
            v.Description,
            v.Duration,
            v.Year,
            t.ID AS TagID,
            t.Value AS TagValue,
            p.ID AS PornstarID,
            p.Value AS PornstarValue
        FROM 
            Videos v
        LEFT JOIN 
            TagVideo tv ON v.ID = tv.VideosID
        LEFT JOIN 
            Tags t ON tv.TagsID = t.ID
        LEFT JOIN 
            PornstarVideo pv ON v.ID = pv.VideosID
        LEFT JOIN 
            Pornstars p ON pv.PornstarsID = p.ID
        WHERE 
            v.ID IN (
                SELECT v1.ID
                FROM Videos v1
                LEFT JOIN TagVideo tv1 ON v1.ID = tv1.VideosID
                LEFT JOIN PornstarVideo pv1 ON v1.ID = pv1.VideosID
                WHERE 
                    (tv1.TagsID IN (@TagIDs) OR @TagIDs IS NULL)
                    AND (pv1.PornstarsID IN (@PornstarIDs) OR @PornstarIDs IS NULL)
            )
        ORDER BY 
        v.ID;
    ";

    private IDbConnection Connection => new MySqlConnection(_connectionString);

    public SearchProvider()
    {
        _connectionString = Environment.GetEnvironmentVariable("BDD_CONNECTION_STRING")
            ?? throw new InvalidOperationException("La variable d'environnement 'BDD_CONNECTION_STRING' est manquante.");
    }

    public async Task<SearchResult> SearchVideosByCriteria(SearchCriteria searchCriteria)
    {
        using var dbConnection = Connection;
        dbConnection.Open();

        var videoDictionary = new Dictionary<int, Video>();

        var videos = await dbConnection.QueryAsync<Video, VideoTag, VideoPornstar, Video>(
            _query,
            (video, tag, pornstar) =>
            {
                if (!videoDictionary.TryGetValue(video.VideoID, out var currentVideo))
                {
                    currentVideo = video;
                    videoDictionary.Add(video.VideoID, currentVideo);
                }

                if (tag != null && !currentVideo.Tags.Any(t => t.TagID == tag.TagID))
                {
                    currentVideo.Tags.Add(tag);
                }

                if (pornstar != null && !currentVideo.Pornstars.Any(p => p.PornstarID == pornstar.PornstarID))
                {
                    currentVideo.Pornstars.Add(pornstar);
                }

                return currentVideo;
            },
            new { TagIDs = searchCriteria.TagsIDS, PornstarIDs = searchCriteria.PornstarsIDS},
            splitOn: "TagID,PornstarID"
        );

        return new SearchResult
        {
            Videos = videoDictionary?.Values?.Select(video => video.Adapt<Core.Videos.Models.Video>())?.ToList() ?? []
        };
    }
}
